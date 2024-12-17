using MatajuApi.Models;
using MatajuApi.Data;

namespace MatajuApi.Data;

public class DataSeederEfCore : IDataSeeder
{
    private readonly AppDbContext _context;

    /// <summary>
    /// 생성자를 이용한 Db 스키마 주입
    /// </summary>
    /// <param name="context"></param>
    public DataSeederEfCore(AppDbContext context)
    {
        _context = context;
    }

    public void SeedAllData()
    {
        SeedHouses();
        SeedUnits();
        SeedUsers();
    }

    public void SeedUsers()
    {
        if (_context.Users.Any())
            return;

        List<User>? users = GenerateUsers();
        _context.Users.AddRange(users);
        _context.SaveChanges();
    }

    public void SeedHouses()
    {
        if (_context.Houses.Any())
            return;

        List<House>? houses = GenerateHouses();
        _context.Houses.AddRange(houses);
        _context.SaveChanges();
    }

    public void SeedUnits()
    {
        if (_context.Units.Any())
            return;

        var random = new Random();
        var units = new List<Unit>();
        foreach (House? house in _context.Houses.ToList())
        {
            int houseId = house.Id;
            int numberOfSizeL = random.Next(5, 9);
            int numberOfSizeM = random.Next(10, 21);
            int numberOfSizeS = random.Next(10, 16);
            units.AddRange(GenerateUnits(house.Id, numberOfSizeL, numberOfSizeM, numberOfSizeS));
        }


        _context.Units.AddRange(units);
        _context.SaveChanges();
    }

    // 공통 로직 재사용
    private List<House> GenerateHouses() => new DataSeederInMemory().GenerateHouses();

    private IEnumerable<Unit> GenerateUnits(int houseId, int small, int medium, int large)
        => new DataSeederInMemory().GenerateUnits(houseId, small, medium, large);

    private List<User> GenerateUsers() => new DataSeederInMemory().GenerateUsers();
}