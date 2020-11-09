using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Global
{
    public partial class emojis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Emoji",
                schema: "Global",
                table: "Players",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emoji",
                schema: "Global",
                table: "Players");
        }
    }
}
