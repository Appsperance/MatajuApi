using Microsoft.AspNetCore.Mvc;
using MatajuApi.Models;
using MatajuApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using MatajuApi.Repositories;

namespace MatajuApi.Controllers
{
  /// <summary>
  /// 유저가입,로그인(토큰발급),유저조회등 유저API 콘트롤러
  /// </summary>
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IRepository<User> _userTableRepo;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="userTableRepo">Program DI로 주입되는 User 레포지토리</param>
    public UserController(IRepository<User> userTableRepo)
    {
      _userTableRepo = userTableRepo;
    }

    /// <summary>
    /// 새로운 사용자 등록
    /// </summary>
    /// <param name="userInput">가입할 유저정보 객체 (Name,Password,Nickname)</param>
    /// <returns>등록성공한 유저정보</returns>
    [HttpPost("register")]
    public IActionResult RegisterUser([FromBody] UserRegisterReqDto userInput)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      // 사용자 이름 중복체크. 409 Conflict 응답
      if (_userTableRepo.Exists(u => u.Name == userInput.Name))
      {
        return Conflict($"유저네임: '{userInput.Name}'은 이미 존재합니다.");
      }

      string hashedPassword = PwdHasher.GenerateHash(userInput.Password, out string salt);

      User newUser = new User { Name = userInput.Name, Password = hashedPassword, Salt = salt, Nickname = userInput.Nickname, Roles = "user" };

      _userTableRepo.Add(newUser);

      return Ok(new { newUser.Name, newUser.Nickname, newUser.Roles });
    }

    /// <summary>
    /// 로그인
    /// </summary>
    /// <param name="loginInput">UserLoginReqDto객체(Name , Password)</param>
    /// <param name="configuration">DI에서 주입되는 IConfiguration 객체</param>
    /// <returns>유저ID,Nickname,로그인토큰,Jwt 공개키</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginReqDto loginInput, [FromServices] IConfiguration configuration)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      // 사용자 검증
      User? user = _userTableRepo.Find(u => u.Name == loginInput.Name).FirstOrDefault();
      if (user == null || !PwdHasher.VerifyHash(loginInput.Password, user.Salt, user.Password))
      {
        return Unauthorized("사용자 name 또는 password가 유효하지 않습니다.");
      }


      string token = JwtHelper.GenerateToken(user, configuration);
      string publicKeyPem = configuration["Jwt:PublicKey"];

      return Ok(new { UserId = user.Id, Nickname = user.Nickname, Token = token, JwtPublicKey = publicKeyPem });
    }

    /// <summary>
    /// 유저조회
    /// </summary>
    /// <param name="id">UserId</param>
    /// <returns>유저정보</returns>
    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult GetUserById(int id)
    {
      User? user = _userTableRepo.GetById(id);
      if (user == null)
      {
        return NotFound("유저를 찾을 수 없습니다.");
      }

      return Ok(new { UserId = user.Id, Name = user.Name, Nickname = user.Nickname, Roles = user.Roles });
    }
  }
}