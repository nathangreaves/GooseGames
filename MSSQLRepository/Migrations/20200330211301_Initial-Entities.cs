using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations
{
    public partial class InitialEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "JustOne");

            migrationBuilder.CreateTable(
                name: "Responses",
                schema: "JustOne",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    RoundId = table.Column<Guid>(nullable: false),
                    Word = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStatuses",
                schema: "JustOne",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    Status = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResponseVotes",
                schema: "JustOne",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    ResponseId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponseVotes_Responses_ResponseId",
                        column: x => x.ResponseId,
                        principalSchema: "JustOne",
                        principalTable: "Responses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                schema: "JustOne",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ActivePlayerId = table.Column<Guid>(nullable: true),
                    SessionId = table.Column<Guid>(nullable: false),
                    WordToGuess = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Outcome = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "JustOne",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    CurrentRoundId = table.Column<Guid>(nullable: true),
                    SessionMasterId = table.Column<Guid>(nullable: true),
                    Score = table.Column<int>(nullable: false)
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
                    Id = table.Column<Guid>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    PlayerNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ConnectionId = table.Column<string>(nullable: true)
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
                name: "IX_Players_SessionId",
                schema: "JustOne",
                table: "Players",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatuses_PlayerId",
                schema: "JustOne",
                table: "PlayerStatuses",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResponseVotes_PlayerId",
                schema: "JustOne",
                table: "ResponseVotes",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseVotes_ResponseId",
                schema: "JustOne",
                table: "ResponseVotes",
                column: "ResponseId");

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
                name: "FK_ResponseVotes_Players_PlayerId",
                schema: "JustOne",
                table: "ResponseVotes",
                column: "PlayerId",
                principalSchema: "JustOne",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Rounds_Players_ActivePlayerId",
                schema: "JustOne",
                table: "Rounds",
                column: "ActivePlayerId",
                principalSchema: "JustOne",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "JustOne",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Sessions_SessionId",
                schema: "JustOne",
                table: "Rounds");

            migrationBuilder.DropTable(
                name: "PlayerStatuses",
                schema: "JustOne");

            migrationBuilder.DropTable(
                name: "ResponseVotes",
                schema: "JustOne");

            migrationBuilder.DropTable(
                name: "Responses",
                schema: "JustOne");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "JustOne");

            migrationBuilder.DropTable(
                name: "Rounds",
                schema: "JustOne");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "JustOne");
        }
    }
}
