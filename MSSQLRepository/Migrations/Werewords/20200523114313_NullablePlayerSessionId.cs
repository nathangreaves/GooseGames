using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Werewords
{
    public partial class NullablePlayerSessionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "Werewords",
                table: "Players");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                schema: "Werewords",
                table: "Players",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "Werewords",
                table: "Players",
                column: "SessionId",
                principalSchema: "Werewords",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "Werewords",
                table: "Players");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                schema: "Werewords",
                table: "Players",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "Werewords",
                table: "Players",
                column: "SessionId",
                principalSchema: "Werewords",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
