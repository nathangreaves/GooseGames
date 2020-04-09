using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations
{
    public partial class CreatedUtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "Sessions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "Rounds",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "ResponseVotes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "Responses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "PlayerStatuses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "Players",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "ResponseVotes");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "PlayerStatuses");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                schema: "JustOne",
                table: "Players");
        }
    }
}
