using Microsoft.AspNetCore.Mvc;
using MatajuApi.Models;
using MatajuApi.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace MatajuApi.Controllers
{
    /// <summary>
    /// 유저가입,로그인(토큰발급),유저조회등 유저API 콘트롤러
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// 임시:  In-memory 스태틱 레포지토리 (TODO: DB로 변경하기)
        /// </summary>
        private static readonly List<User> Users = new();

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
            if (Users.Any(u => u.Name == userInput.Name))
            {
                return Conflict($"유저네임: '{userInput.Name}'은 이미 존재합니다.");
            }

            string hashedPassword = PwdHasher.GenerateHash(userInput.Password, out string salt);

            // Create new user
            User newUser = new User
                           {
                               // 임시ID 증가: TODO: DB에 위임하기
                               Id = Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1,
                               Name = userInput.Name,
                               Password = hashedPassword,
                               Salt = salt,
                               Nickname = userInput.Nickname,
                               Roles = "user"
                           };

            // Add to storage
            Users.Add(newUser);

            return Ok(new
                      {
                          newUser.Name,
                          newUser.Nickname,
                          newUser.Roles
                      });
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
            User? user = Users.FirstOrDefault(u => u.Name == loginInput.Name);
            if (user == null || !PwdHasher.VerifyHash(loginInput.Password, user.Salt, user.Password))
            {
                return Unauthorized("사용자 name 또는 password가 유효하지 않습니다.");
            }


            string token = JwtHelper.GenerateToken(user, configuration);
            string publicKeyPem = configuration["Jwt:PublicKey"];

            return Ok(new
                      {
                          UserId = user.Id,
                          Nickname = user.Nickname,
                          Token = token,
                          JwtPublicKey = publicKeyPem
                      });
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
            User? user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("유저를 찾을 수 없습니다.");
            }

            return Ok(new
                      {
                          UserId = user.Id,
                          Name = user.Name,
                          Nickname = user.Nickname,
                          Roles = user.Roles
                      });
        }
    }
}