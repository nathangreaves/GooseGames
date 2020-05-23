using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Werewords
{
    public partial class PasswordUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "Werewords",
                table: "Sessions",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                schema: "Werewords",
                table: "Sessions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Password",
                schema: "Werewords",
                table: "Sessions",
                column: "Password",
                unique: true,
                filter: "[Password] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_Password",
                schema: "Werewords",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                schema: "Werewords",
                table: "Sessions");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "Werewords",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
