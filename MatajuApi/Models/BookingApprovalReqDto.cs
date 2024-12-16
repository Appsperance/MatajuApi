using System.ComponentModel.DataAnnotations;

namespace MatajuApi.Models;

/// <summary>
/// 유닛 예약 승인 요청 DTO 모델 (관리자 API)
/// </summary>
public class BookingApprovalReqDto
{
    /// <summary>
    /// 확인된 비용 지불 날짜
    /// </summary>
    [Required]
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// 확인된 비용 지불 수단
    /// </summary>
    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// 관리자 메모
    /// </summary>
    public string AdminNote { get; set; } = string.Empty;
}