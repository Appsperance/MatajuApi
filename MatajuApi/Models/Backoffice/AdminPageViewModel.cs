namespace MatajuApi.Models;

public class AdminPageViewModel
{
  public List<Booking> PendingBookings { get; set; } = new();
  public List<Booking> CompletedBookings { get; set; } = new();
  public List<Booking> RejectedBookings { get; set; } = new();
}