using Microsoft.AspNetCore.Mvc;
using MatajuApi.Models;
using Microsoft.AspNetCore.Authorization;
using MatajuApi.Repositories;

namespace MatajuApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitController : ControllerBase
{
  private readonly IRepository<House> _houseTableRepo;
  private readonly IRepository<Unit> _unitRepo;

  /// <summary>
  /// 생성자 
  /// </summary>
  /// <param name="houseTableRepo">Program DI로 주입되는 레포지토리</param>
  /// <param name="unitRepo">Program DI로 주입되는 레포지토리</param>
  public UnitController(IRepository<House> houseTableRepo, IRepository<Unit> unitRepo)
  {
    _houseTableRepo = houseTableRepo;
    _unitRepo = unitRepo;
  }

  /// <summary>
  /// 특정 창고지점의 모든 유닛 목록
  /// </summary>
  /// <param name="houseId">창고지점 ID</param>
  /// <returns>창고지점 하나의 모든 Unit 목록</returns>
  [HttpGet("house/{houseId:int}")]
  public IActionResult GetUnitsByHouseId(int houseId)
  {
    var house = _houseTableRepo.GetById(houseId);
    if (house == null)
      return NotFound($"House ID: {houseId}에 해당하는 창고지점을 찾을 수 없구만유.");

    var units = _unitRepo.Find(u => u.HouseId == houseId).Select(u => new UnitResponseDto()
                                                                      {
                                                                        Id = u.Id, HouseId = u.HouseId, Size = u.Size, Status = u.Status, StartDate = u.StartDate,
                                                                        EndDate = u.EndDate, Price = u.Size switch
                                                                                                     {
                                                                                                       UnitSize.S => house.PriceS,
                                                                                                       UnitSize.M => house.PriceM,
                                                                                                       UnitSize.L => house.PriceL,
                                                                                                       _ => 999990 //switch default
                                                                                                     }
                                                                      });

    return Ok(units);
  }

  /// <summary>
  /// 유닛 하나에대한 정보
  /// </summary>
  /// <param name="unitId">찾을 유닛 ID</param>
  /// <returns>유닛 정보</returns>
  [HttpGet("{unitId:int}")]
  public IActionResult GetUnitById(int unitId)
  {
    var unit = _unitRepo.GetById(unitId);
    if (unit == null)
      return NotFound($"Unit ID {unitId}에 해당하는 유닛을 찾을 수 없구만유.");

    var house = _houseTableRepo.GetById(unit.HouseId);
    if (house == null)
      return NotFound($"Unit ID {unitId}가 속한 House를 찾을 수 없는데유? DB정보가 잘 못 된 것같아유~");

    var unitDto = new UnitResponseDto
                  {
                    Id = unit.Id, HouseId = unit.HouseId, Size = unit.Size, Status = unit.Status, StartDate = unit.StartDate, EndDate = unit.EndDate,
                    Price = unit.Size switch { UnitSize.S => house.PriceS, UnitSize.M => house.PriceM, UnitSize.L => house.PriceL, _ => 999990 }
                  };

    return Ok(unitDto);
  }

  /// <summary>
  /// 여러 Unit 정보를 한번에 조회
  /// </summary>
  /// <param name="unitIds">Unit Id 목록</param>
  /// <returns></returns>
  [HttpPost("units")]
  public IActionResult GetUnitsByIds([FromBody] List<int> unitIds)
  {
    var units = _unitRepo.Find(u => unitIds.Contains(u.Id));
    if (!units.Any())
      return NotFound("요청된 Unit ID들에 해당하는 유닛을 찾을 수 없습니다.");

    var unitDtos = units.Select(unit =>
                                {
                                  var house = _houseTableRepo.GetById(unit.HouseId);
                                  if (house == null) return null;

                                  return new UnitResponseDto()
                                         {
                                           Id = unit.Id, HouseId = unit.HouseId, Size = unit.Size, Status = unit.Status, StartDate = unit.StartDate, EndDate = unit.EndDate,
                                           Price = unit.Size switch { UnitSize.S => house.PriceS, UnitSize.M => house.PriceM, UnitSize.L => house.PriceL, _ => 0 }
                                         };
                                }).Where(u => u != null).ToList();

    return Ok(unitDtos);
  }
}