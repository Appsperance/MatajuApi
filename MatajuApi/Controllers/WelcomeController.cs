using Microsoft.AspNetCore.Mvc;

namespace MatajuApi.Controllers
{
  [ApiController]
  [Route("api")]
  public class WelcomeController : ControllerBase
  {
    [HttpGet]
    public IActionResult Welcome()
    {
      return Ok("어서 와유~ '마타유' API 백엔드가 실행중이구유. 뭘 맡아드릴까유?");
    }
  }
}