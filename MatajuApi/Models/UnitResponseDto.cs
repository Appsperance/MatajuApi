namespace MatajuApi.Models;

/// <summary>
/// 유닛정보 요청에대한 응답 DTO 모델
/// </summary>
public class UnitResponseDto
{
    public int Id { get; set; } // Unit ID
    public int HouseId { get; set; } // 소속된 House ID
    public UnitSize Size { get; set; } // Unit 크기 (S, M, L)
    public UnitStatus Status { get; set; } // 현재 상태 (Available, InUse 등)
    public DateTime? StartDate { get; set; } // 예약 시작일
    public DateTime? EndDate { get; set; } // 예약 종료일

    // 가격 정보 : 소속 House에 따라 다름)
    public int Price { get; set; }
}