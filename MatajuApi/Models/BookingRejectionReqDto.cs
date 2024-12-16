using System.ComponentModel.DataAnnotations;

namespace MatajuApi.Models;

/// <summary>
/// 유닛 예약 거부 요청 DTO 모델 (관리자 API)
/// </summary>
public class BookingRejectionReqDto
{
    /// <summary>
    /// 관리자 메모
    /// </summary>
    [Required]
    public string AdminNote { get; set; } = string.Empty;
}