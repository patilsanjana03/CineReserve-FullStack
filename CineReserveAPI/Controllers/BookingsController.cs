using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineReserveAPI.Data;
using CineReserveAPI.DTOs;
using CineReserveAPI.Models;

namespace CineReserveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;
    public BookingsController(AppDbContext db) { _db = db; }

    // ═══════════════════════════════════════════════════════════
    // POST /api/bookings — CORE ENDPOINT
    // Uses Serializable transaction + UNIQUE constraint
    // to prevent double-booking (concurrency control)
    // ═══════════════════════════════════════════════════════════
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Book([FromBody] CreateBookingDto dto)
    {
        if (dto.Seats == null || dto.Seats.Count == 0)
            return BadRequest(new { message = "No seats selected" });

        // Get logged-in user ID from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        // Open Serializable transaction — prevents phantom reads during concurrent bookings
        using var transaction = await _db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable);

        try
        {
            // Fetch user and showtime inside transaction
            var user = await _db.Users.FindAsync(userId);
            var showtime = await _db.Showtimes.FindAsync(dto.ShowtimeId);

            if (user == null) return Unauthorized();
            if (showtime == null) return NotFound(new { message = "Showtime not found" });

            // VIP rows (G, H) get 1.5x price
            var vipRows = new[] { "G", "H" };

            // Calculate total
            decimal total = dto.Seats.Sum(seat =>
                vipRows.Contains(seat.RowLetter.ToUpper())
                    ? showtime.BasePrice * 1.5m
                    : showtime.BasePrice);

            // Check credit balance
            if (user.CreditBalance < total)
                return BadRequest(new { message = $"Insufficient balance. Need ₹{total}, have ₹{user.CreditBalance}" });

            // Generate unique booking reference
            var bookingRef = $"CR-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

            // Create seat booking records
            var seatBookings = dto.Seats.Select(seat => new SeatBooking
            {
                ShowtimeId = dto.ShowtimeId,
                RowLetter = seat.RowLetter.ToUpper(),
                SeatNumber = seat.SeatNumber,
                UserId = userId,
                BookingRef = bookingRef,
                SeatPrice = vipRows.Contains(seat.RowLetter.ToUpper())
                    ? showtime.BasePrice * 1.5m
                    : showtime.BasePrice,
                BookedAt = DateTime.UtcNow
            }).ToList();

            _db.SeatBookings.AddRange(seatBookings);
            user.CreditBalance -= total;

            // SaveChangesAsync will throw DbUpdateException if UNIQUE constraint violated
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new BookingResponseDto
            {
                BookingRef = bookingRef,
                TotalPaid = total,
                SeatsBooked = dto.Seats.Count,
                RemainingBalance = user.CreditBalance
            });
        }
        catch (DbUpdateException ex)
            when (ex.InnerException?.Message.Contains("UNIQUE") == true ||
                  ex.InnerException?.Message.Contains("duplicate") == true ||
                  ex.InnerException?.Message.Contains("UX_SeatBooking") == true)
        {
            // CONCURRENCY CAUGHT HERE
            await transaction.RollbackAsync();
            return Conflict(new
            {
                message = "One or more seats were just booked by another user. Please reselect your seats."
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { message = "Booking failed. Please try again.", detail = ex.Message });
        }
    }

    // GET /api/bookings/mine — user's booking history
    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var bookings = await _db.SeatBookings
            .Where(sb => sb.UserId == userId)
            .Include(sb => sb.Showtime)
                .ThenInclude(st => st!.Movie)
            .OrderByDescending(sb => sb.BookedAt)
            .AsNoTracking()
            .ToListAsync();

        // Group by BookingRef for display
        var grouped = bookings
            .GroupBy(sb => sb.BookingRef)
            .Select(g => new
            {
                BookingRef = g.Key,
                MovieTitle = g.First().Showtime?.Movie?.Title,
                ShowDate = g.First().Showtime?.ShowDate,
                ShowTime = g.First().Showtime?.ShowTime.ToString(@"hh\:mm"),
                HallName = g.First().Showtime?.HallName,
                Seats = g.Select(s => $"{s.RowLetter}-{s.SeatNumber}").ToList(),
                TotalPaid = g.Sum(s => s.SeatPrice),
                BookedAt = g.First().BookedAt
            });

        return Ok(grouped);
    }
}