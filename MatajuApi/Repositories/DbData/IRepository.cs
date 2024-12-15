using System.Linq.Expressions;

namespace MatajuApi.Repositories;

/// <summary>
///  DB 레코드에대해 제네릭한 CRUD 작업을 추상화하는 인터페이스
/// </summary>
/// <typeparam name="T">테이블 엔티티 모델 클래스</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Id에 해당하는 하나의 행 읽기
    /// </summary>
    /// <param name="id">테이블의 Id칼럼(PK) 값</param>
    /// <returns></returns>
    T? GetById(int id);

    /// <summary>
    /// 조건에 따라 여러 레코드 읽기
    /// </summary>
    /// <param name="predicate">조건을 나타내는 람다식 - 표현식 트리(Expression Trees)</param>
    /// <returns>조건에 맞는 레코드 목록</returns>
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 조건에 따라 코드 존재 여부 확인
    /// </summary>
    /// <param name="predicate">조건을 나타내는 람다식 - 표현식 트리(Expression Trees)</param>
    /// <returns>조건에 맞는 레코드 존재 여부</returns>
    bool Exists(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 하나의 레코드 추가
    /// </summary>
    /// <param name="entity">테이블의 레코드로 입력할 엔티티 모델 객체</param>
    /// <returns>엔티티 모델 객체</returns>
    T Add(T entity);

    /// <summary>
    /// 하나의 레코드 수정 
    /// </summary>
    /// <param name="entity">수정할 레코드와 같은 엔티티객체</param>
    void Update(T entity);

    /// <summary>
    /// ID로 엔터티 삭제 (하나의 레코드 삭제)
    /// </summary>
    /// <param name="id">테이블의 Id칼럼(PK) 값</param>
    void Delete(int id);

#region Data Seeding : //TODO: 삭제

    /// <summary>
    /// 모든 데이터를 삭제
    /// </summary>
    void Clear();

    /// <summary>
    /// 정해진 초기 데이터 시딩
    /// </summary>
    /// <param name="entities">초기화할 데이터 목록</param>
    void SetInitialData(IEnumerable<T> entities);

#endregion
}