using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Global
{
    public partial class GlobalEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Global");

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "Global",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    SessionMasterId = table.Column<Guid>(nullable: true),
                    Game = table.Column<int>(nullable: false),
                    GameSessionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "Global",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: true),
                    PlayerNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "Global",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_SessionId",
                schema: "Global",
                table: "Players",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Password",
                schema: "Global",
                table: "Sessions",
                column: "Password",
                unique: true,
                filter: "[Password] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionMasterId",
                schema: "Global",
                table: "Sessions",
                column: "SessionMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Players_SessionMasterId",
                schema: "Global",
                table: "Sessions",
                column: "SessionMasterId",
                principalSchema: "Global",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "Global",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "Global");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "Global");
        }
    }
}
