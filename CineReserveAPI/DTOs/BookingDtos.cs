namespace CineReserveAPI.DTOs;

public class SeatDto
{
    public string RowLetter { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
}

public class CreateBookingDto
{
    public int ShowtimeId { get; set; }
    public List<SeatDto> Seats { get; set; } = new();
}

public class BookingResponseDto
{
    public string BookingRef { get; set; } = string.Empty;
    public decimal TotalPaid { get; set; }
    public int SeatsBooked { get; set; }
    public decimal RemainingBalance { get; set; }
}