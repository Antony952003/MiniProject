using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBookingAPI.Migrations
{
    public partial class paymentstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowtimeSeats_Showtimes_ShowtimeId",
                table: "ShowtimeSeats");

            migrationBuilder.AlterColumn<int>(
                name: "CancellationId",
                table: "ShowtimeSeats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BookingId",
                table: "ShowtimeSeats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowtimeSeats_Showtimes_ShowtimeId",
                table: "ShowtimeSeats",
                column: "ShowtimeId",
                principalTable: "Showtimes",
                principalColumn: "ShowtimeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowtimeSeats_Showtimes_ShowtimeId",
                table: "ShowtimeSeats");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Payments");

            migrationBuilder.AlterColumn<int>(
                name: "CancellationId",
                table: "ShowtimeSeats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BookingId",
                table: "ShowtimeSeats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowtimeSeats_Showtimes_ShowtimeId",
                table: "ShowtimeSeats",
                column: "ShowtimeId",
                principalTable: "Showtimes",
                principalColumn: "ShowtimeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
