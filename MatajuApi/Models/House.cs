using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatajuApi.Models;

/// <summary>
/// 창고 지점(Warehouse)
/// </summary>
public class House
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("HouseId")]
    public int Id { get; set; }

    /// <summary>
    /// 주소
    /// </summary>
    public string Add { get; set; } = string.Empty;

    /// <summary>
    /// 지역
    /// </summary>
    public string Province { get; set; } = string.Empty;

    public int PriceS { get; set; }
    public int PriceM { get; set; }
    public int PriceL { get; set; }
}