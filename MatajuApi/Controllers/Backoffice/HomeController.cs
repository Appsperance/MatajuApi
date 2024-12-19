using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MatajuApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace MatajuApi.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)] // Swagger에서 제외
  public class HomeController : Controller
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
      _httpClientFactory = httpClientFactory;
      _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
      return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(string username, string password)
    {
      HttpClient? httpClient = _httpClientFactory.CreateClient();

      // 현재 요청의 호스트 가져오기
      HttpRequest? request = _httpContextAccessor.HttpContext.Request;
      httpClient.BaseAddress = new Uri($"{request.Scheme}://{request.Host}");

      var loginInput = new { Name = username, Password = password };
      StringContent? content = new StringContent(JsonSerializer.Serialize(loginInput), Encoding.UTF8, "application/json");

      // 상대 경로를 사용하여 API 요청
      HttpResponseMessage? response = await httpClient.PostAsync("/api/user/login", content);

      if (response.IsSuccessStatusCode)
      {
        string? responseContent = await response.Content.ReadAsStringAsync();
        LoginResponse? apiResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // "Bearer " 접두사 추가 후 쿠키에 저장
        string bearerToken = $"Bearer {apiResponse.Token}";
        Response.Cookies.Append("token", bearerToken, new CookieOptions { HttpOnly = true, Path = "/", Expires = DateTimeOffset.UtcNow.AddDays(1) });

        return RedirectToAction("Admin");
      }

      ViewData["Error"] = "ID 또는 Password가 맞지 않습니다";
      return View("Index");
    }

    [HttpGet("/admin")]
    [Authorize(Roles = "admin")]
    public IActionResult Admin()
    {
      return View();
    }
  }
}