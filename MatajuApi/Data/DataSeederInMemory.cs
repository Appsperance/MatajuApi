using MatajuApi.Models;
using MatajuApi.Repositories;
using MatajuApi.Helpers;

namespace MatajuApi.Data;

/// <summary>
/// House와 Unit 초기데이터를 레포지토리에 기록
/// </summary>
public class DataSeederInMemory : IDataSeeder
{
    private readonly IRepository<House> _houseRepo;
    private readonly IRepository<Unit> _unitRepo;
    private readonly IRepository<User> _userRepo;

    /// <summary>
    /// 기본 생성자: 공통로직 재사용 용도
    /// </summary>
    public DataSeederInMemory()
    {
    }

    /// <summary>
    /// 생성자를 통한 레포지토리 주입
    /// </summary>
    /// <param name="houseRepo">House 테이블 레포지토리</param>
    /// <param name="unitRepo">Unit 테이블 레포지토리</param>
    /// <param name="userRepo">User 테이블 레포지토리</param>
    public DataSeederInMemory(IRepository<House> houseRepo, IRepository<Unit> unitRepo, IRepository<User> userRepo)
    {
        _houseRepo = houseRepo;
        _unitRepo = unitRepo;
        _userRepo = userRepo;
    }


    public void SeedAllData()
    {
        SeedUsers();
        SeedHouses();
        SeedUnits();
    }

    public void SeedHouses()
    {
        if (_houseRepo.Find(h => true).Any())
            return;

        _houseRepo.SetInitialData(GenerateHouses());
    }

    public void SeedUnits()
    {
        if (_unitRepo.Find(u => true).Any())
            return;

        var random = new Random();
        var units = new List<Unit>();

        foreach (var house in _houseRepo.Find(h => true))
        {
            int houseId = house.Id;
            int numberOfSizeL = random.Next(5, 9);
            int numberOfSizeM = random.Next(10, 21);
            int numberOfSizeS = random.Next(10, 16);
            units.AddRange(GenerateUnits(house.Id, numberOfSizeL, numberOfSizeM, numberOfSizeS));
        }

        _unitRepo.SetInitialData(units);
    }

    public void SeedUsers()
    {
        if (_userRepo.Find(u => true).Any())
            return;
        _userRepo.SetInitialData(GenerateUsers());
    }


    /// <summary>
    /// User 테이블에 하나의 유저와 관리자를 생성한다.
    /// </summary>
    /// <returns></returns>
    internal List<User> GenerateUsers()
    {
        return new List<User>
               {
                   new User
                   {
                       Name = "string", Password = PwdHasher.GenerateHash("string", out var randomSalt1), Salt = randomSalt1, Nickname = "첫손님",
                       Roles = "user"
                   },
                   new User
                   {
                       Name = "admin", Password = PwdHasher.GenerateHash("vapor", out var randomSalt2), Salt = randomSalt2, Nickname = "창고지기",
                       Roles = "admin"
                   }
               };
    }

#region 공통로직: House, Unit 테이블 레코드 생성

    /// <summary>
    /// House 테이블(창고지점) 레코드 생성 
    /// </summary>
    /// <returns></returns>
    internal List<House> GenerateHouses()
    {
        return new List<House>
               {
                   new House { Add = "서울특별시 종로구 세종로 1번지", Province = "서울특별시", PriceS = 60000, PriceM = 120000, PriceL = 299900 },
                   new House { Add = "부산광역시 해운대구 해운대로 620번길 15", Province = "부산광역시", PriceS = 55000, PriceM = 99000, PriceL = 250000 },
                   new House { Add = "대구광역시 수성구 동대구로 85번길 12", Province = "대구광역시", PriceS = 55000, PriceM = 99000, PriceL = 250000 },
                   new House { Add = "인천광역시 연수구 송도동 23-1", Province = "인천광역시", PriceS = 46000, PriceM = 88000, PriceL = 200000 },
                   new House { Add = "광주광역시 북구 우치로 200번길 5", Province = "광주광역시", PriceS = 46000, PriceM = 88000, PriceL = 200000 },
                   new House { Add = "대전광역시 서구 둔산대로 100번길 10", Province = "대전광역시", PriceS = 46000, PriceM = 88000, PriceL = 200000 },
                   new House { Add = "울산광역시 남구 삼산로 95번길 20", Province = "울산광역시", PriceS = 42000, PriceM = 70000, PriceL = 160000 },
                   new House { Add = "경기도 수원시 영통구 매탄로 33번길 18", Province = "경기도", PriceS = 46000, PriceM = 88000, PriceL = 200000 },
                   new House { Add = "강원도 춘천시 봉의동 중앙로 50번길 7", Province = "강원도", PriceS = 55000, PriceM = 99000, PriceL = 250000 },
                   new House { Add = "충청북도 청주시 상당구 서원로 17번길 5", Province = "충청북도", PriceS = 35000, PriceM = 65000, PriceL = 160000 },
                   new House { Add = "충청남도 천안시 서북구 백석로 25번길 9", Province = "충청남도", PriceS = 55000, PriceM = 99000, PriceL = 250000 },
                   new House { Add = "전라북도 전주시 완산구 효자동3가 23-10", Province = "전라북도", PriceS = 35000, PriceM = 65000, PriceL = 160000 },
                   new House { Add = "전라남도 여수시 학동 20번길 8", Province = "전라남도", PriceS = 35000, PriceM = 65000, PriceL = 160000 },
                   new House { Add = "경상북도 포항시 북구 양덕로 12번길 11", Province = "경상북도", PriceS = 35000, PriceM = 65000, PriceL = 160000 },
                   new House { Add = "경상남도 창원시 성산구 중앙대로 30번길 15", Province = "경상남도", PriceS = 35000, PriceM = 65000, PriceL = 160000 },
               };
    }

    /// <summary>
    /// House별 Unit 테이블의 레코드를 크기별 갯수만큼 생성한다.
    /// </summary>
    /// <param name="houseId"></param>
    /// <param name="small">UnitSize.S 사이즈 갯수</param>
    /// <param name="medium">UnitSize.M 사이즈 갯수</param>
    /// <param name="large">UnitSize.L 사이즈 갯수</param>
    /// <returns></returns>
    internal IEnumerable<Unit> GenerateUnits(int houseId, int small, int medium, int large)
    {
        for (int i = 0; i < small; i++)
            yield return new Unit { HouseId = houseId, Size = UnitSize.S, Status = UnitStatus.Available };
        for (int i = 0; i < medium; i++)
            yield return new Unit { HouseId = houseId, Size = UnitSize.M, Status = UnitStatus.Available };
        for (int i = 0; i < large; i++)
            yield return new Unit { HouseId = houseId, Size = UnitSize.L, Status = UnitStatus.Available };
    }

#endregion
}