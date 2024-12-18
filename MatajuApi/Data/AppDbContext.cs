using MatajuApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatajuApi.Data;

/// <summary>
/// 인메모리 데이터베이스 스키마 클래스
/// </summary>
/// <remarks>in-memory representation of actual DB Schema - EF Core DbContext 클래스</remarks>
public class AppDbContext : DbContext
{
  /// <summary>
  /// 생성자로 Db 커넥션 정보 주입 후 부모 생성자를 통해 DbContext 객체를 생성한다.
  /// </summary>
  /// <param name="options">부모생성자에 전달할 DbContextOptions<T> 옵션 </param>
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  //테이블
  public DbSet<User> Users { get; set; }
  public DbSet<House> Houses { get; set; }
  public DbSet<Unit> Units { get; set; }
  public DbSet<Booking> Bookings { get; set; }
}