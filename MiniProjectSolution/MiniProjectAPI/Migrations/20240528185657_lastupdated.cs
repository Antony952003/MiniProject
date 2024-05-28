using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBookingAPI.Migrations
{
    public partial class lastupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EarnedDate",
                table: "UserPoints",
                newName: "LastUpdated");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "UserPoints",
                newName: "EarnedDate");
        }
    }
}
