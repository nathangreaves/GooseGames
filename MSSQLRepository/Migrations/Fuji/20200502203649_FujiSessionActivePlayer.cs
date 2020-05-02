using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Fuji
{
    public partial class FujiSessionActivePlayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                schema: "Fuji",
                table: "Sessions");

            migrationBuilder.AddColumn<Guid>(
                name: "ActivePlayerId",
                schema: "Fuji",
                table: "Sessions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ActivePlayerId",
                schema: "Fuji",
                table: "Sessions",
                column: "ActivePlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Players_ActivePlayerId",
                schema: "Fuji",
                table: "Sessions",
                column: "ActivePlayerId",
                principalSchema: "Fuji",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Players_ActivePlayerId",
                schema: "Fuji",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_ActivePlayerId",
                schema: "Fuji",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ActivePlayerId",
                schema: "Fuji",
                table: "Sessions");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                schema: "Fuji",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
