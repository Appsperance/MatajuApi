using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MatajuApi.Models;

namespace MatajuApi.Controllers
{
  public class HomeController : Controller
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
      return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(string username, string password)
    {
      // API 호출
      var httpClient = _httpClientFactory.CreateClient();
      var loginInput = new { Name = username, Password = password };
      var content = new StringContent(JsonSerializer.Serialize(loginInput), Encoding.UTF8, "application/json");
      var response = await httpClient.PostAsync("https://your-api-domain/api/user/login", content);

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // JWT 토큰을 쿠키에 저장
        Response.Cookies.Append("token", apiResponse.Token, new CookieOptions { HttpOnly = true, Path = "/", Expires = DateTimeOffset.UtcNow.AddDays(1) });

        return RedirectToAction("Admin");
      }

      // 로그인 실패 처리
      ViewData["Error"] = "Invalid username or password";
      return View("index");
    }

    [HttpGet("/admin")]
    public IActionResult Admin()
    {
      return View();
    }
  }
}