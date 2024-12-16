using System.ComponentModel.DataAnnotations;

namespace MatajuApi.Models;

/// <summary>
/// 유닛 예약 요청 DTO 모델
/// </summary>
public class BookUnitReqDto
{
    /// <summary>
    /// 예약 신청자 User Id
    /// </summary>
    [Required]
    public int UserId { get; set; }

    [Required]
    public int HouseId { get; set; }

    // 예약하려는 유닛 크기(S, M, L)
    [Required]
    public UnitSize UnitSize { get; set; }

    /// <summary>
    /// // 예약 시작일. "2024-12-31"
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 예약 기간 (28~3650일)
    /// </summary>
    [Required]
    [Range(28, 3650, ErrorMessage = "예약 기간은 최소 28일에서 최대 3650일이어야 합니다.")]
    public int DurationDays { get; set; }

    /// <summary>
    /// 예약자의 메시지
    /// </summary>
    public string UserNote { get; set; } = string.Empty;
}

/*
 {
   "userId": 1,
   "houseId": 10,
   "unitSize": "M",
   "startDate": "2024-12-20",
   "durationDays": 30,
   "UserNote": "폭탄을 설치할 자리를 예약하고 싶습니다"
 }
 */