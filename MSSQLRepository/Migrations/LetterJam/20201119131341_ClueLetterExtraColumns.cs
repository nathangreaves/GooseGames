using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.LetterJam
{
    public partial class ClueLetterExtraColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWildCard",
                schema: "LetterJam",
                table: "ClueLetters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LetterIndex",
                schema: "LetterJam",
                table: "ClueLetters",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWildCard",
                schema: "LetterJam",
                table: "ClueLetters");

            migrationBuilder.DropColumn(
                name: "LetterIndex",
                schema: "LetterJam",
                table: "ClueLetters");
        }
    }
}
