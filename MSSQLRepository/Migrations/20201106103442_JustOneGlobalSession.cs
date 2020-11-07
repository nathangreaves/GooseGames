using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations
{
    public partial class JustOneGlobalSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStatuses_Players_PlayerId",
                schema: "JustOne",
                table: "PlayerStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Players_PlayerId",
                schema: "JustOne",
                table: "Responses");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseVotes_Players_PlayerId",
                schema: "JustOne",
                table: "ResponseVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Players_ActivePlayerId",
                schema: "JustOne",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Sessions_SessionId",
                schema: "JustOne",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "JustOne",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "JustOne");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "JustOne");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_ActivePlayerId",
                schema: "JustOne",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_SessionId",
                schema: "JustOne",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_ResponseVotes_PlayerId",
                schema: "JustOne",
                table: "ResponseVotes");

            migrationBuilder.DropIndex(
                name: "IX_Responses_PlayerId",
                schema: "JustOne",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStatuses_PlayerId",
                schema: "JustOne",
                table: "PlayerStatuses");

            migrationBuilder.Sql("DELETE FROM JustOne.ResponseVotes", false);
            migrationBuilder.Sql("DELETE FROM JustOne.Responses", false);
            migrationBuilder.Sql("DELETE FROM JustOne.PlayerStatuses", false);
            migrationBuilder.Sql("DELETE FROM JustOne.Rounds", false);

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                schema: "JustOne",
                table: "Rounds",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                schema: "JustOne",
                table: "PlayerStatuses",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                schema: "JustOne",
                table: "PlayerStatuses",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "JustOne",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    CurrentRoundId = table.Column<Guid>(nullable: true),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Rounds_CurrentRoundId",
                        column: x => x.CurrentRoundId,
                        principalSchema: "JustOne",
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_GameId",
                schema: "JustOne",
                table: "Rounds",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_CurrentRoundId",
                schema: "JustOne",
                table: "Games",
                column: "CurrentRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Games_GameId",
                schema: "JustOne",
                table: "Rounds",
                column: "GameId",
                principalSchema: "JustOne",
                principalTable: "Games",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Games_GameId",
                schema: "JustOne",
                table: "Rounds");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "JustOne");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_GameId",
                schema: "JustOne",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "GameId",
                schema: "JustOne",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                schema: "JustOne",
                table: "PlayerStatuses");

            migrationBuilder.DropColumn(
                name: "GameId",
                schema: "JustOne",
                table: "PlayerStatuses");

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "JustOne",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentRoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    SessionMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Rounds_CurrentRoundId",
                        column: x => x.CurrentRoundId,
                        principalSchema: "JustOne",
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "JustOne",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerNumber = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "JustOne",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_ActivePlayerId",
                schema: "JustOne",
                table: "Rounds",
                column: "ActivePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_SessionId",
                schema: "JustOne",
                table: "Rounds",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseVotes_PlayerId",
                schema: "JustOne",
                table: "ResponseVotes",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_PlayerId",
                schema: "JustOne",
                table: "Responses",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatuses_PlayerId",
                schema: "JustOne",
                table: "PlayerStatuses",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_SessionId",
                schema: "JustOne",
                table: "Players",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CurrentRoundId",
                schema: "JustOne",
                table: "Sessions",
                column: "CurrentRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionMasterId",
                schema: "JustOne",
                table: "Sessions",
                column: "SessionMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStatuses_Players_PlayerId",
                schema: "JustOne",
                table: "PlayerStatuses",
                column: "PlayerId",
                principalSchema: "JustOne",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Players_PlayerId",
                schema: "JustOne",
                table: "Responses",
                column: "PlayerId",
                principalSchema: "JustOne",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseVotes_Players_PlayerId",
                schema: "JustOne",
                table: "ResponseVotes",
                column: "PlayerId",
                principalSchema: "JustOne",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Players_ActivePlayerId",
                schema: "JustOne",
                table: "Rounds",
                column: "ActivePlayerId",
                principalSchema: "JustOne",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Sessions_SessionId",
                schema: "JustOne",
                table: "Rounds",
                column: "SessionId",
                principalSchema: "JustOne",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Players_SessionMasterId",
                schema: "JustOne",
                table: "Sessions",
                column: "SessionMasterId",
                principalSchema: "JustOne",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
