using System.ComponentModel.DataAnnotations;

namespace MatajuApi.Models;

/// <summary>
/// 사용자 로그인 토큰 요청 DTO 모델
/// </summary>
public class UserLoginReqDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}