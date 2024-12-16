using MatajuApi.Models;
using MatajuApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatajuApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] TODO: 활성화 및 Role 확인
public class AdminController : ControllerBase
{
    private readonly IRepository<Unit> _unitRepo;
    private readonly IRepository<Booking> _bookingRepo;


    /// <summary>
    /// 생성자 - Program DI Container로부터 레포지토리 주입
    /// </summary>
    /// <param name="unitRepo">유닛 테이블 레포지토리</param>
    /// <param name="bookingRepo">예약 테이블 레포지토리</param>
    public AdminController(IRepository<Unit> unitRepo, IRepository<Booking> bookingRepo)
    {
        _unitRepo = unitRepo;
        _bookingRepo = bookingRepo;
    }

    /// <summary>
    /// 예약 승인
    /// </summary>
    /// <param name="bookingId">승인할 Booking ID</param>
    /// <param name="request">승인에 필요한 지불 정보</param>
    /// <returns>승인 결과</returns>
    [HttpPost("approve-booking/{bookingId:int}")]
    public IActionResult ApproveBooking(int bookingId, [FromBody] BookingApprovalReqDto request)
    {
        Booking? booking = _bookingRepo.GetById(bookingId);
        if (booking == null)
        {
            return NotFound($"예약번호 {bookingId}에 해당하는 예약을 찾을 수 없습니다.");
        }

        Unit? unit = _unitRepo.GetById(booking.UnitId);
        if (unit == null)
        {
            return NotFound($"예약번호 {bookingId}에 해당하는 Unit을 찾을 수 없습니다.");
        }

        // Booking 승인 처리
        booking.Status = BookingStatus.Completed;
        booking.PaymentDate = request.PaymentDate;
        booking.PaymentMethod = request.PaymentMethod;
        booking.AdminNote = request.AdminNote;
        _bookingRepo.Update(booking);

        // Unit 상태 업데이트
        unit.Status = UnitStatus.InUse;
        _unitRepo.Update(unit);

        return Ok(new
                  {
                      Message = $"예약번호 {bookingId}이 성공적으로 승인되었습니다.",
                      Booking = booking,
                      Unit = unit
                  });
    }

    /// <summary>
    /// 예약 거부
    /// </summary>
    /// <param name="bookingId">거부할 Booking ID</param>
    /// <param name="request">거부사유(필수)가 포함된 데이터</param>
    /// <returns>거부 결과</returns>
    [HttpPost("reject-booking/{bookingId:int}")]
    public IActionResult RejectBooking(int bookingId, [FromBody] BookingRejectionReqDto request)
    {
        Booking? booking = _bookingRepo.GetById(bookingId);
        if (booking == null)
        {
            return NotFound($"예약번호 {bookingId}에 해당하는 예약을 찾을 수 없습니다.");
        }

        Unit? unit = _unitRepo.GetById(booking.UnitId);
        if (unit == null)
        {
            return NotFound($"예약번호 {bookingId}에 해당하는 Unit을 찾을 수 없습니다.");
        }

        booking.Status = BookingStatus.Rejected;
        booking.AdminNote = request.AdminNote;
        _bookingRepo.Update(booking);

        // Unit 상태 초기화
        unit.Status = UnitStatus.Available;
        unit.UserId = null;
        unit.StartDate = null;
        unit.EndDate = null;
        _unitRepo.Update(unit);

        return Ok(new
                  {
                      Message = $"예약 번호 {bookingId}이 거부되었습니다.",
                      Booking = booking,
                      Unit = unit
                  });
    }
}