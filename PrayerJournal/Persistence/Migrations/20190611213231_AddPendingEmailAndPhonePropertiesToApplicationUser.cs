using Microsoft.EntityFrameworkCore.Migrations;

namespace PrayerJournal.Persistence.Migrations
{
    public partial class AddPendingEmailAndPhonePropertiesToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedPendingEmail",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingEmail",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingPhoneNumber",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedPendingEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PendingEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PendingPhoneNumber",
                table: "AspNetUsers");
        }
    }
}
