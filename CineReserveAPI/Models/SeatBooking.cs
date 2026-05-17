namespace CineReserveAPI.Models;

public class SeatBooking
{
    public int Id { get; set; }
    public int ShowtimeId { get; set; }
    public string RowLetter { get; set; } = string.Empty;   // A, B, C ... H
    public int SeatNumber { get; set; }                      // 1 to 10
    public int UserId { get; set; }
    public string BookingRef { get; set; } = string.Empty;   // CR-20260516-1234
    public decimal SeatPrice { get; set; }                   // price for this seat
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;

    public Showtime? Showtime { get; set; }
    public User? User { get; set; }
}