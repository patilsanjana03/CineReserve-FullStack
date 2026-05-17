using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CineReserveAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    PosterUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreditBalance = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Showtimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ShowDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShowTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    HallName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Showtimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Showtimes_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeatBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowtimeId = table.Column<int>(type: "int", nullable: false),
                    RowLetter = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookingRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BookedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeatBookings_Showtimes_ShowtimeId",
                        column: x => x.ShowtimeId,
                        principalTable: "Showtimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatBookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Description", "Duration", "Genre", "PosterUrl", "Rating", "Title" },
                values: new object[,]
                {
                    { 1, "A thief who steals corporate secrets through dream-sharing technology.", 148, "Sci-Fi / Thriller", "https://picsum.photos/seed/inception/300/450", 8.8m, "Inception" },
                    { 2, "A team of explorers travel through a wormhole in space to ensure humanity's survival.", 169, "Sci-Fi / Adventure", "https://picsum.photos/seed/interstellar/300/450", 8.6m, "Interstellar" },
                    { 3, "Batman faces the Joker, a criminal mastermind who creates chaos in Gotham City.", 152, "Action / Drama", "https://picsum.photos/seed/darkknight/300/450", 9.0m, "The Dark Knight" },
                    { 4, "The Avengers assemble once more to reverse Thanos' actions.", 181, "Action / Adventure", "https://picsum.photos/seed/avengers/300/450", 8.4m, "Avengers: Endgame" }
                });

            migrationBuilder.InsertData(
                table: "Showtimes",
                columns: new[] { "Id", "BasePrice", "HallName", "MovieId", "ShowDate", "ShowTime" },
                values: new object[,]
                {
                    { 1, 250m, "Hall A", 1, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 10, 0, 0, 0) },
                    { 2, 300m, "Hall B", 1, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 14, 30, 0, 0) },
                    { 3, 350m, "Hall A", 1, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 18, 0, 0, 0) },
                    { 4, 280m, "Hall C", 2, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 11, 0, 0, 0) },
                    { 5, 320m, "Hall A", 2, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 16, 0, 0, 0) },
                    { 6, 260m, "Hall B", 3, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 12, 30, 0, 0) },
                    { 7, 380m, "Hall C", 3, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 20, 0, 0, 0) },
                    { 8, 300m, "Hall A", 4, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 15, 0, 0, 0) },
                    { 9, 350m, "Hall B", 4, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 19, 30, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeatBookings_ShowtimeId",
                table: "SeatBookings",
                column: "ShowtimeId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatBookings_UserId",
                table: "SeatBookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_SeatBooking_Showtime_Row_Seat",
                table: "SeatBookings",
                columns: new[] { "ShowtimeId", "RowLetter", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_MovieId",
                table: "Showtimes",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeatBookings");

            migrationBuilder.DropTable(
                name: "Showtimes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
