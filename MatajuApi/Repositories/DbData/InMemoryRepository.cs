using System.Linq.Expressions;
using System.Reflection;

namespace MatajuApi.Repositories;

/// <summary>
/// DB레포지토리로 전환 전에 사용하는 인메모리 레포지토리 (아직 DB를 연결하지 않았을 때 사용).
/// T(엔티티) 테이블에대한 작업 수행 
/// </summary>
/// <remarks>주의: DI에서 싱글톤 주입</remarks>
/// <typeparam name="T">테이블 엔티티 모델 클래스</typeparam>
public class InMemoryRepository<T> : IRepository<T> where T : class
{
  /// <summary>
  /// 테이블의 모든 행(row) 데이터
  /// </summary>
  private readonly List<T> _allEntities = new();
  /// <summary>
  /// Id 필드 Auto-Increment 
  /// </summary>
  private int _nextId = 1;

  public T? GetById(int id)
  {
    PropertyInfo? property = GetIdProperty();

    return _allEntities.FirstOrDefault(entity => (int)property.GetValue(entity, null)! == id);
  }

  public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
  {
    return _allEntities.AsQueryable().Where(predicate).ToList();
  }

  public bool Exists(Expression<Func<T, bool>> predicate) => _allEntities.AsQueryable().Any(predicate);

  public T Add(T entity)
  {
    PropertyInfo? property = GetIdProperty();

    property.SetValue(entity, _nextId++);
    _allEntities.Add(entity);
    return entity;
  }

  public void Update(T entity)
  {
    PropertyInfo? property = GetIdProperty();
    int id = (int)property.GetValue(entity, null)!;

    T? existing = GetById(id);
    if (existing != null)
    {
      _allEntities.Remove(existing);
      _allEntities.Add(entity);
    }
  }

  public void Delete(int id)
  {
    T? existing = GetById(id);
    if (existing != null)
      _allEntities.Remove(existing);
  }

#region Data Seeding :

  public void Clear()
  {
    _allEntities.Clear();
    _nextId = 1; // Auto-Increment 초기화
  }

  public void SetInitialData(IEnumerable<T> entities)
  {
    Clear();
    foreach (var entity in entities)
    {
      Add(entity);
    }
  }

#endregion

  /// <summary>
  /// 엔티티 모델인 T 클래스의 Id 프라퍼티(변수)에 대한 메타정보를 반환한다. Id라는 프라파터가 반드시 존재해야하며, 없다면 예외를 던진다. 
  /// </summary>
  /// <remarks>리플렉션을 사용해서 T의 프라퍼티 정보를 읽는다</remarks>
  /// <returns>T 클래스에서 변수이름이 Id인 프라퍼티의 메타 정보객체</returns>
  /// <exception cref="InvalidOperationException">엔티티 모델인 T 클래스에 'Id' 프라퍼티가 없을때 예외출력</exception>
  private PropertyInfo GetIdProperty()
  {
    PropertyInfo? property = typeof(T).GetProperty("Id");
    if (property == null)
      throw new InvalidOperationException($"{nameof(T)}에는 Id 프라티가 존재하지 않음");
    return property;
  }
}