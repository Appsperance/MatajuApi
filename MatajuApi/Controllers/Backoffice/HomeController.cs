using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MatajuApi.Models;
using MatajuApi.Repositories;
using System.Text;

namespace MatajuApi.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  public class HomeController : Controller
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<Booking> _bookingRepo;

    public HomeController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IRepository<Booking> bookingRepo // Booking 레포지토리 주입
    )
    {
      _httpClientFactory = httpClientFactory;
      _httpContextAccessor = httpContextAccessor;
      _bookingRepo = bookingRepo;
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
      HttpRequest? request = _httpContextAccessor.HttpContext.Request;
      httpClient.BaseAddress = new Uri($"{request.Scheme}://{request.Host}");

      var loginInput = new { Name = username, Password = password };
      StringContent? content = new StringContent(System.Text.Json.JsonSerializer.Serialize(loginInput), Encoding.UTF8, "application/json");

      HttpResponseMessage? response = await httpClient.PostAsync("/api/user/login", content);

      if (response.IsSuccessStatusCode)
      {
        string? responseContent = await response.Content.ReadAsStringAsync();
        LoginResponse? apiResponse =
          System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        string bearerToken = $"Bearer {apiResponse.Token}";
        Response.Cookies.Append("token", bearerToken, new CookieOptions { HttpOnly = true, Path = "/", Expires = DateTimeOffset.UtcNow.AddDays(1) });

        return RedirectToAction("AdminPage");
      }

      ViewData["Error"] = "ID 또는 Password가 맞지 않습니다";
      return View("Index");
    }

    [HttpGet("/admin/page")]
    [Authorize(Roles = "admin")]
    public IActionResult AdminPage([FromServices] IRepository<Booking> bookingRepo)
    {
      var pending = bookingRepo.Find(b => b.Status == BookingStatus.Pending).ToList();
      var completed = bookingRepo.Find(b => b.Status == BookingStatus.Completed).ToList();
      var rejected = bookingRepo.Find(b => b.Status == BookingStatus.Rejected).ToList();

      var vm = new AdminPageViewModel { PendingBookings = pending, CompletedBookings = completed, RejectedBookings = rejected };

      return View("AdminPage", vm);
    }

    [HttpPost("/admin/page/process")]
    [Authorize(Roles = "admin")]
    public IActionResult Process([FromServices] IRepository<Booking> _bookingRepo, [FromServices] IRepository<Unit> unitRepo, int bookingId, string adminNote,
                                 DateTime? paymentDate, PaymentMethod? paymentMethod)
    {
      var booking = _bookingRepo.GetById(bookingId);
      if (booking == null || booking.Status != BookingStatus.Pending)
      {
        return NotFound("해당 예약을 찾을 수 없거나 펜딩 상태가 아닙니다.");
      }

      if (Request.Form.ContainsKey("Approve"))
      {
        // 승인 시 결제 정보 필수
        if (!paymentDate.HasValue || !paymentMethod.HasValue)
        {
          ModelState.AddModelError("", "승인 시 결제일자 및 결제수단이 필요합니다.");

          var pendingBookings = _bookingRepo.Find(b => b.Status == BookingStatus.Pending).ToList();
          var completedBookings = _bookingRepo.Find(b => b.Status == BookingStatus.Completed).ToList();

          var vm = new AdminPageViewModel { PendingBookings = pendingBookings, CompletedBookings = completedBookings };

          return View("AdminPage", vm);
        }

        // 승인 처리
        booking.Status = BookingStatus.Completed;
        booking.PaymentDate = DateTime.UtcNow;
        booking.PaymentMethod = paymentMethod.Value;
        booking.AdminNote = adminNote ?? "";
        _bookingRepo.Update(booking);

        var unit = unitRepo.GetById(booking.UnitId);
        if (unit != null && unit.Status == UnitStatus.PendingCheckIn)
        {
          unit.Status = UnitStatus.InUse;
          unitRepo.Update(unit);
        }
      } else if (Request.Form.ContainsKey("Reject"))
      {
        // 거부 처리
        booking.Status = BookingStatus.Rejected;
        booking.AdminNote = adminNote ?? "";
        _bookingRepo.Update(booking);

        var unit = unitRepo.GetById(booking.UnitId);
        if (unit != null && unit.Status == UnitStatus.PendingCheckIn)
        {
          unit.Status = UnitStatus.Available;
          unit.UserId = null;
          unit.StartDate = null;
          unit.EndDate = null;
          unitRepo.Update(unit);
        }
      }

      // 처리 완료 후 페이지 갱신
      return RedirectToAction("AdminPage");
    }

    [HttpPost("/admin/page/logout")]
    [Authorize(Roles = "admin")]
    public IActionResult Logout()
    {
      HttpContext.Response.Cookies.Delete("token");
      return Redirect("/User/Login");
    }
  }
}