using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Global
{
    public partial class NullableGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Game",
                schema: "Global",
                table: "Sessions",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Game",
                schema: "Global",
                table: "Sessions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
