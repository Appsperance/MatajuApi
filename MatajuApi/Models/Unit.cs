using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatajuApi.Models;

/// <summary>
/// 창고 캐비닛 유닛에대한 엔티티 모델
/// </summary>
public class Unit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("UnitId")]
    public int Id { get; set; }

    /// <summary>
    /// 예약자 (예약신청자, 사용중인자, 비움신청자) 
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 유닛이 소속된 창고지점(Warehouse)
    /// </summary>
    public int HouseId { get; set; }

    public UnitSize Size { get; set; }

    /// <summary>
    /// 유닛의 현재상태. 
    /// </summary>
    /// <remarks>DB에는 문자열로 저장</remarks>
    [Column(TypeName = "varchar(25)")]
    public UnitStatus Status { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 유닛크기
/// </summary>
public enum UnitSize
{
    S, M, L
}

/// <summary>
/// 유닛의 현재 상태
/// </summary>
public enum UnitStatus
{
    /// <summary>
    /// 예약가능
    /// </summary>
    Available,

    /// <summary>
    /// 사용중(예약됨)
    /// </summary>
    InUse,

    /// <summary>
    /// 예약에대해 관리자 승인 대기중
    /// </summary>
    PendingCheckIn,

    /// <summary>
    /// 비움에대해 관리자 승인 대기중
    /// </summary>
    PendingCheckOut,

    /// <summary>
    /// 사용불가: 고장 및 분쟁, 기타상태 
    /// </summary>
    InTrouble
}