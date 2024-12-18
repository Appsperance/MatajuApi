using System.Linq.Expressions;
using MatajuApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MatajuApi.Repositories;

/// <summary>
/// EF Core 기반의 데이터베이스 레포지토리 구현체
/// </summary>
/// <typeparam name="T">테이블 엔티티 모델 클래스</typeparam>
public class DbEfCoreRepository<T> : IRepository<T> where T : class
{
  private readonly AppDbContext _context;
  private readonly DbSet<T> _dbSet;

  /// <summary>
  /// 생성자 매개변수를 통한 AppDbCOntext 객체 DI 주입
  /// </summary>
  /// <param name="context">DB스키마 객체</param>
  public DbEfCoreRepository(AppDbContext context)
  {
    _context = context;
    _dbSet = _context.Set<T>();
  }

  public T? GetById(int id) => _dbSet.Find(id);
  public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).ToList();
  public bool Exists(Expression<Func<T, bool>> predicate) => _dbSet.Any(predicate);

  public T Add(T entity)
  {
    _dbSet.Add(entity);
    _context.SaveChanges();
    return entity;
  }

  public void Update(T entity)
  {
    _dbSet.Update(entity);
    _context.SaveChanges();
  }

  public void Delete(int id)
  {
    var entity = GetById(id);
    if (entity != null)
    {
      _dbSet.Remove(entity);
      _context.SaveChanges();
    }
  }

  public void Clear()
  {
    _dbSet.RemoveRange(_dbSet);
    _context.SaveChanges();
  }

  public void SetInitialData(IEnumerable<T> entities)
  {
    Clear();
    _dbSet.AddRange(entities);
    _context.SaveChanges();
  }
}