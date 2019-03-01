using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PrayerJournal.Persistence.Migrations
{
    public partial class AddedTokenInvalidationTimeToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TokensInvalidBefore",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokensInvalidBefore",
                table: "AspNetUsers");
        }
    }
}
