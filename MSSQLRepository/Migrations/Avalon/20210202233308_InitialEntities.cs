using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Avalon
{
    public partial class InitialEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Avalon");

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "Avalon",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    GodPlayerId = table.Column<Guid>(nullable: false),
                    NumberOfPlayers = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameRoles",
                schema: "Avalon",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    RoleEnum = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameRoles_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "Avalon",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerIntel",
                schema: "Avalon",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    IntelType = table.Column<int>(nullable: false),
                    IntelPlayerId = table.Column<Guid>(nullable: true),
                    IntelNumber = table.Column<int>(nullable: true),
                    RoleKnowsYou = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerIntel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerIntel_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "Avalon",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStates",
                schema: "Avalon",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    GameRoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerStates_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "Avalon",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerStates_GameRoles_GameRoleId",
                        column: x => x.GameRoleId,
                        principalSchema: "Avalon",
                        principalTable: "GameRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameRoles_GameId",
                schema: "Avalon",
                table: "GameRoles",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerIntel_GameId",
                schema: "Avalon",
                table: "PlayerIntel",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerIntel_PlayerId",
                schema: "Avalon",
                table: "PlayerIntel",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_GameId",
                schema: "Avalon",
                table: "PlayerStates",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_GameRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                column: "GameRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_PlayerId",
                schema: "Avalon",
                table: "PlayerStates",
                column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerIntel",
                schema: "Avalon");

            migrationBuilder.DropTable(
                name: "PlayerStates",
                schema: "Avalon");

            migrationBuilder.DropTable(
                name: "GameRoles",
                schema: "Avalon");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "Avalon");
        }
    }
}
