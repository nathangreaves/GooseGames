using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Werewords
{
    public partial class TokensSpent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SoCloseSpent",
                schema: "Werewords",
                table: "Rounds",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WayOffSpent",
                schema: "Werewords",
                table: "Rounds",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoCloseSpent",
                schema: "Werewords",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "WayOffSpent",
                schema: "Werewords",
                table: "Rounds");
        }
    }
}
