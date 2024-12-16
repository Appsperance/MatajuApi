using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatajuApi.Models;

/// <summary>
/// 예약신청 현황 테이블 엔티티 모델
/// </summary>
public class Booking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("BookingId")]
    public int Id { get; set; }

    /// <summary>
    /// 예약 신청한 사용자 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 할당된 Unit ID
    /// </summary>
    public int UnitId { get; set; }

    /// <summary>
    /// 예약 신청한 날짜
    /// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// 처리 업무 종류 (예약신청, 퇴실신청 등)
    /// </summary>
    [Column(TypeName = "varchar(25)")]
    public BookingType Type { get; set; }

    /// <summary>
    /// 예약기간 계산 금액 (Unit Price / 30 * days)
    /// </summary>
    public int Charge { get; set; }

    /// <summary>
    /// 비용 지불된 날짜 
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// 비용 지불 수단
    /// </summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// 예약 처리 상태
    /// </summary>
    [Column(TypeName = "varchar(25)")]
    public BookingStatus Status { get; set; }

    /// <summary>
    /// 예약자의 메시지
    /// </summary>
    public string UserNote { get; set; } = string.Empty;

    /// <summary>
    /// 관리자 메모
    /// </summary>
    public string AdminNote { get; set; } = string.Empty;
}

/// <summary>
/// 예약 처리 업무 종류
/// </summary>
public enum BookingType
{
    CheckIn, // 예약신청
    CheckOut // 퇴실신청
}

/// <summary>
/// 예약 처리 상태
/// </summary>
public enum BookingStatus
{
    Pending, // 펜딩 상태
    Rejected, //반려
    Completed, // 완료됨
}

/// <summary>
/// 비용 지불 수단
/// </summary>
public enum PaymentMethod
{
    Cash,
    Card,
    VaporPay,
    BitCoin
}