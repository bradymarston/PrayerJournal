using Microsoft.EntityFrameworkCore.Migrations;

namespace PrayerJournal.Persistence.Migrations
{
    public partial class AddedSugestPasswordChangeToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SuggestPasswordChange",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuggestPasswordChange",
                table: "AspNetUsers");
        }
    }
}
