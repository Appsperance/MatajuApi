using MatajuApi.Models;
using MatajuApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MatajuApi.Helpers;

namespace MatajuApi.Controllers;

/// <summary>
/// 유닛 예약 API 콘트롤러
/// </summary>
[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IRepository<User> _userTableRepo;
    private readonly IRepository<Unit> _unitTableRepo;
    private readonly IRepository<House> _houseTableRepo;
    private readonly IRepository<Booking> _bookingTableRepo;

    /// <summary>
    /// 생성자 -  DI Container에서 매개변수로 레포지토리 주입
    /// </summary>
    /// <param name="userTableRepo">User 레포지토리</param>
    /// <param name="houseTableRepo">Unit 레포지토리</param>
    /// <param name="unitTableRepo">Unit 레포지토리</param>
    public BookingController(IRepository<User> userTableRepo,
                             IRepository<House> houseTableRepo,
                             IRepository<Unit> unitTableRepo,
                             IRepository<Booking> bookingRepo)
    {
        _userTableRepo = userTableRepo;
        _unitTableRepo = unitTableRepo;
        _houseTableRepo = houseTableRepo;
        _bookingTableRepo = bookingRepo;
    }

    /// <summary>
    /// 예약 조회
    /// </summary>
    /// <param name="id">예약번호: Booking 테이블 레코드 Id</param>
    /// <returns>예약정보</returns>
    [HttpGet("{id:int}")]
    public IActionResult GetBookingById(int id)
    {
        Booking? booking = _bookingTableRepo.GetById(id);
        if (booking == null)
        {
            return NotFound($"Booking ID {id}에 해당하는 예약을 찾을 수 없습니다.");
        }

        return Ok(booking);
    }

    /// <summary>
    /// 예약신청
    /// </summary>
    /// <param name="request">요청 JSON</param>
    /// <returns>예약 정보</returns>
    /// <exception cref="ArgumentException"></exception>
    [HttpPost]
    public IActionResult BookUnit([FromBody] BookUnitReqDto request)
    {
        User? user = _userTableRepo.GetById(request.UserId);
        if (user == null)
        {
            return BadRequest($"UserId {request.UserId}에 해당하는 사용자를 찾을 수 없습니다.");
        }

        House? house = _houseTableRepo.GetById(request.HouseId);
        if (house == null)
        {
            return BadRequest($"HouseId {request.HouseId}에 해당하는 창고 지점을 찾을 수 없습니다.");
        }

        // 사이즈에 맞는 유닛이 남은게 있는지 확인
        Unit? availableUnit = _unitTableRepo.Find(u =>
                                                      u.HouseId == request.HouseId &&
                                                      u.Size == request.UnitSize &&
                                                      u.Status == UnitStatus.Available).FirstOrDefault();
        if (availableUnit == null)
        {
            return NotFound($"HouseId {request.HouseId}에는 남은 {request.UnitSize.ToString()}사이즈의 유닛이 없습니다.");
        }

        // Booking 엔티티 생성 및 저장
        int unitPrice = request.UnitSize switch
                        {
                            UnitSize.S => house.PriceS,
                            UnitSize.M => house.PriceM,
                            UnitSize.L => house.PriceL,
                            _ => throw new ArgumentException("유효하지 않은 UnitSize입니다.", nameof(request.UnitSize))
                        };

        int totalPrice = Pricing.CalculateBookingCharge(unitPrice, request.DurationDays);

        DateTime requestDate = DateTime.UtcNow;
        DateTime endDate = request.StartDate.AddDays(request.DurationDays);

        Booking booking = new Booking
                          {
                              UserId = request.UserId,
                              UnitId = availableUnit.Id,
                              RequestDate = requestDate,
                              Type = BookingType.CheckIn,
                              Charge = totalPrice,
                              PaymentDate = null,
                              PaymentMethod = null,
                              Status = BookingStatus.Pending
                          };

        _bookingTableRepo.Add(booking);

        // 유닛 엔티티 업데이트
        availableUnit.UserId = request.UserId;
        availableUnit.Status = UnitStatus.PendingCheckIn;
        availableUnit.StartDate = request.StartDate;
        availableUnit.EndDate = endDate;
        _unitTableRepo.Update(availableUnit);

        return CreatedAtAction(nameof(BookUnit), new { id = booking.Id }, new
                                                                          {
                                                                              Message = "예약이 성공적으로 생성되었습니다.",
                                                                              BookingId = booking.Id,
                                                                              TotalPrice = totalPrice,
                                                                              BookingDetails = booking
                                                                          });
    }
}