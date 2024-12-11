using Microsoft.AspNetCore.Mvc;
using MatajuApi.Models;
using MatajuApi.Helpers;

namespace MatajuApi.Controllers
{
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
            var newUser = new User
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginReqDto loginInput, [FromServices] IConfiguration configuration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 사용자 검증
            var user = Users.FirstOrDefault(u => u.Name == loginInput.Name);
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
    }
}