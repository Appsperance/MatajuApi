using MatajuApi.Models;
using MatajuApi.Repositories;

namespace MatajuApi.Helpers;

/// <summary>
/// House와 Unit 초기데이터를 레포지토리에 기록
/// </summary>
public static class DataSeeder
{
  /// <summary>
  /// 초기데이터 생성. House 테이블 레포지토리 레코드와 그에 맞춘 Unit 테이블 레포지토리의 레코드를 생성한다 
  /// </summary>
  /// <param name="houseRepo">House 테이블 레포지토리</param>
  /// <param name="unitRepo">Unit 테이블 레포지토리</param>
  /// <param name="userRepo">User 테이블 레포지토리</param>
  public static void SeedData(IRepository<House> houseRepo, IRepository<Unit> unitRepo, IRepository<User> userRepo)
  {
    SeedHouses(houseRepo);
    SeedUnits(houseRepo, unitRepo);
    SeedUsers(userRepo);
  }

  /// <summary>
  /// 창고 지점 데이터 시딩
  /// </summary>
  /// <param name="houseRepo">House 테이블 레포지토리</param>
  private static void SeedHouses(IRepository<House> houseRepo)
  {
    if (houseRepo.Find(h => true).Any()) return; // 이미 데이터가 있으면 초기화하지 않음

    var sampleHouses = new List<House>
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

    houseRepo.SetInitialData(sampleHouses);
  }

  /// <summary>
  /// 창고지점별 창고유닛 데이터 시딩
  /// </summary>
  /// <param name="houseRepo">House 테이블 레포지토리</param>
  /// <param name="unitRepo">Unit 테이블 레포지토리</param>
  private static void SeedUnits(IRepository<House> houseRepo, IRepository<Unit> unitRepo)
  {
    if (unitRepo.Find(u => true).Any()) return;

    var random = new Random();
    var sampleUnits = new List<Unit>();

    foreach (var house in houseRepo.Find(h => true))
    {
      int houseId = house.Id;

      int numberOfSizeL = random.Next(5, 9);
      sampleUnits.AddRange(GenerateUnits(houseId, UnitSize.L, numberOfSizeL));

      int numberOfSizeM = random.Next(10, 21);
      sampleUnits.AddRange(GenerateUnits(houseId, UnitSize.M, numberOfSizeM));

      int numberOfSizeS = random.Next(10, 16);
      sampleUnits.AddRange(GenerateUnits(houseId, UnitSize.S, numberOfSizeS));
    }

    unitRepo.SetInitialData(sampleUnits);
  }

  /// <summary>
  /// 특정 사이즈에대한 Unit 엔티티를 count만큼 생성한다. 
  /// </summary>
  /// <param name="houseId">유닛이 속한 House 레코드 Id</param>
  /// <param name="size">유닛크기(S,M,L)</param>
  /// <param name="count">이 사이즈의 유닛 총 갯수</param>
  /// <returns>Unit 모델 객체 목록</returns>
  private static IEnumerable<Unit> GenerateUnits(int houseId, UnitSize size, int count)
  {
    for (int i = 0; i < count; i++)
    {
      yield return new Unit { HouseId = houseId, Size = size, Status = UnitStatus.Available, StartDate = null, EndDate = null };
    }
  }

  /// <summary>
  /// 임시 유저 시딩. 하나의 디폴트 User와 Admin을 추가한다.
  /// TODO: DB전환시 삭제하기
  /// </summary>
  /// <param name="userRepo">User 테이블 레포지토리</param>
  private static void SeedUsers(IRepository<User> userRepo)
  {
    if (userRepo.Find(u => true).Any()) return;

    // 유저 생성
    var users = new List<User>();

    // 디폴트 유저 추가
    string userPasswordHash = PwdHasher.GenerateHash("string", out string userSalt);
    users.Add(new User
              {
                Name = "string", Password = userPasswordHash, //"string"
                Salt = userSalt, Nickname = "첫손님", Roles = "user"
              });

    // Admin 유저 추가
    string adminPasswordHash = PwdHasher.GenerateHash("vapor", out string adminSalt);
    users.Add(new User
              {
                Name = "admin", Password = adminPasswordHash, //"vapor"
                Salt = adminSalt, Nickname = "창고지기", Roles = "admin"
              });

    userRepo.SetInitialData(users);
  }
}