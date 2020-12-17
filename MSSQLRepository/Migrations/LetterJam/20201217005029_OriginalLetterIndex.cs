using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.LetterJam
{
    public partial class OriginalLetterIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OriginalLetterIndex",
                schema: "LetterJam",
                table: "LetterCards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalLetterIndex",
                schema: "LetterJam",
                table: "LetterCards");
        }
    }
}
