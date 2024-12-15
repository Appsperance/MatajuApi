using MatajuApi.Models;
using MatajuApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatajuApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HouseController : ControllerBase
{
    private readonly IRepository<House> _houseTableRepo;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="houseTableRepo">Program DI로 주입되는 User 레포지토리</param>
    public HouseController(IRepository<House> houseTableRepo)
    {
        _houseTableRepo = houseTableRepo;
    }


    /// <summary>
    /// ID로 창고 지점 조회
    /// </summary>
    /// <param name="id">지점ID - HouseId</param>
    /// <returns>해당 ID의 지점 정보</returns>
    [HttpGet("{id:int}")]
    public IActionResult GetHouseById(int id)
    {
        House? house = _houseTableRepo.GetById(id);
        if (house == null)
        {
            return NotFound("해당 ID에 해당하는 창고를 찾을 수 없습니다.");
        }

        return Ok(house);
    }

    /// <summary>
    /// 모든 창고(House) 목록 조회
    /// </summary>
    /// <returns>창고 목록</returns>
    [HttpGet("all")]
    public IActionResult GetAllHouses()
    {
        var houses = _houseTableRepo.Find(h => true); // 모든 엔티티를 가져옴
        return Ok(houses);
    }
}