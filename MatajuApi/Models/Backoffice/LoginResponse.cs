namespace MatajuApi.Models;

public class LoginResponse
{
  public int UserId { get; set; }
  public string Nickname { get; set; }
  public string Token { get; set; }
  public string JwtPublicKey { get; set; }
}