using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.LetterJam
{
    public partial class GameNumberOfPlayers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfPlayers",
                schema: "LetterJam",
                table: "Games",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPlayers",
                schema: "LetterJam",
                table: "Games");
        }
    }
}
