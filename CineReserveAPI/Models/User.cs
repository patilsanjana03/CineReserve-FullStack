namespace CineReserveAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // "User" or "Admin"
    public decimal CreditBalance { get; set; } = 5000m; // mock credit

    public ICollection<SeatBooking> Bookings { get; set; } = new List<SeatBooking>();
}