using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Werewords
{
    public partial class IsMayorColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMayor",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMayor",
                schema: "Werewords",
                table: "PlayerRoundInformation");
        }
    }
}
