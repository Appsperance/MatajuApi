using MatajuApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatajuApi.Data;

/// <summary>
/// EF Core 데이터베이스 스키마 클래스
/// </summary>
/// <remarks>in-memory representation of actual DB Schema - EF Core DbContext 클래스</remarks>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    //테이블
    public DbSet<User> Users { get; set; }
    public DbSet<House> Houses { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}