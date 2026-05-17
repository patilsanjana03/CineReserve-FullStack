namespace CineReserveAPI.Models;

public class Showtime
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public DateTime ShowDate { get; set; }
    public TimeSpan ShowTime { get; set; }
    public string HallName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }

    public Movie? Movie { get; set; }
    public ICollection<SeatBooking> SeatBookings { get; set; } = new List<SeatBooking>();
}