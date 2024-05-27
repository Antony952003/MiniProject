using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBookingAPI.Migrations
{
    public partial class snackquantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Snacks");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BookingSnacks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BookingSnacks");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Snacks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
