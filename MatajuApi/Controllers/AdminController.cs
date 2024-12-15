using MatajuApi.Models;
using MatajuApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MatajuApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IRepository<House> _houseTableRepo;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="houseTableRepo">Program DI로 주입되는 House 레포지토리</param>
    public AdminController(IRepository<House> houseTableRepo)
    {
        _houseTableRepo = houseTableRepo;
    }

    /// <summary>
    /// House 레포지토리에 임의의 데이터 추가 (개발용 임시 엔드포인트).
    /// </summary>
    /// <returns>추가된 House 데이터 목록</returns>
    [HttpPost("seed-houses")]
    public IActionResult SeedHouses()
    {
        // 임의 데이터 생성
        var sampleHouses = new List<House>
                           {
                               new House { Add = "서울특별시 종로구 세종로 1번지", Province = "서울특별시", PriceS = 35000, PriceM = 78000, PriceL = 119000 },
                               new House { Add = "부산광역시 해운대구 해운대로 620번길 15", Province = "부산광역시", PriceS = 24000, PriceM = 36000, PriceL = 69000 },
                               new House { Add = "대구광역시 수성구 동대구로 85번길 12", Province = "대구광역시", PriceS = 24000, PriceM = 36000, PriceL = 69000 },
                               new House { Add = "인천광역시 연수구 송도동 23-1", Province = "인천광역시", PriceS = 23000, PriceM = 32000, PriceL = 60000 },
                               new House { Add = "광주광역시 북구 우치로 200번길 5", Province = "광주광역시", PriceS = 23000, PriceM = 32000, PriceL = 60000 },
                               new House { Add = "대전광역시 서구 둔산대로 100번길 10", Province = "대전광역시", PriceS = 24000, PriceM = 36000, PriceL = 69000 },
                               new House { Add = "울산광역시 남구 삼산로 95번길 20", Province = "울산광역시", PriceS = 23000, PriceM = 32000, PriceL = 60000 },
                               new House { Add = "경기도 수원시 영통구 매탄로 33번길 18", Province = "경기도", PriceS = 23000, PriceM = 32000, PriceL = 60000 },
                               new House { Add = "강원도 춘천시 봉의동 중앙로 50번길 7", Province = "강원도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "충청북도 청주시 상당구 서원로 17번길 5", Province = "충청북도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "충청남도 천안시 서북구 백석로 25번길 9", Province = "충청남도", PriceS = 35000, PriceM = 78000, PriceL = 119000 },
                               new House { Add = "전라북도 전주시 완산구 효자동3가 23-10", Province = "전라북도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "전라남도 여수시 학동 20번길 8", Province = "전라남도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "경상북도 포항시 북구 양덕로 12번길 11", Province = "경상북도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "경상남도 창원시 성산구 중앙대로 30번길 15", Province = "경상남도", PriceS = 21000, PriceM = 28000, PriceL = 45000 }
                           };

        _houseTableRepo.SetInitialData(sampleHouses);

        return Ok(new
                  {
                      Message = $"{sampleHouses.Count}개의 House 데이터가 추가되었구유.",
                      Houses = sampleHouses
                  });
    }
}