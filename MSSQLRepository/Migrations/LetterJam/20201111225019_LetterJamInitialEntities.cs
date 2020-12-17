using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.LetterJam
{
    public partial class LetterJamInitialEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LetterJam");

            migrationBuilder.CreateTable(
                name: "LetterCards",
                schema: "LetterJam",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    Letter = table.Column<string>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: true),
                    NonPlayerCharacterId = table.Column<Guid>(nullable: true),
                    LetterIndex = table.Column<int>(nullable: true),
                    PlayerLetterGuess = table.Column<string>(nullable: true),
                    BonusLetter = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonPlayerCharacters",
                schema: "LetterJam",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    Emoji = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PlayerNumber = table.Column<int>(nullable: false),
                    NumberOfLettersRemaining = table.Column<int>(nullable: false),
                    CurrentLetterId = table.Column<Guid>(nullable: true),
                    ClueReleased = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonPlayerCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonPlayerCharacters_LetterCards_CurrentLetterId",
                        column: x => x.CurrentLetterId,
                        principalSchema: "LetterJam",
                        principalTable: "LetterCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStates",
                schema: "LetterJam",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    Status = table.Column<Guid>(nullable: true),
                    StatusDescription = table.Column<string>(nullable: true),
                    CurrentLetterId = table.Column<Guid>(nullable: true),
                    CurrentLetterIndex = table.Column<int>(nullable: true),
                    NumberOfCluesGiven = table.Column<int>(nullable: false),
                    OriginalWordLength = table.Column<int>(nullable: false),
                    FinalWordLength = table.Column<int>(nullable: true),
                    Successful = table.Column<bool>(nullable: true),
                    Points = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerStates_LetterCards_CurrentLetterId",
                        column: x => x.CurrentLetterId,
                        principalSchema: "LetterJam",
                        principalTable: "LetterCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                schema: "LetterJam",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    RoundNumber = table.Column<int>(nullable: false),
                    RoundStatus = table.Column<int>(nullable: false),
                    ClueGiverPlayerId = table.Column<Guid>(nullable: true),
                    ClueId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "LetterJam",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    RedCluesRemaining = table.Column<int>(nullable: false),
                    GreenCluesRemaining = table.Column<int>(nullable: false),
                    LockedCluesRemaining = table.Column<int>(nullable: false),
                    GameStatus = table.Column<int>(nullable: false),
                    CurrentRoundId = table.Column<Guid>(nullable: true),
                    Points = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Rounds_CurrentRoundId",
                        column: x => x.CurrentRoundId,
                        principalSchema: "LetterJam",
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_CurrentRoundId",
                schema: "LetterJam",
                table: "Games",
                column: "CurrentRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_SessionId",
                schema: "LetterJam",
                table: "Games",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterCards_GameId",
                schema: "LetterJam",
                table: "LetterCards",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_NonPlayerCharacters_CurrentLetterId",
                schema: "LetterJam",
                table: "NonPlayerCharacters",
                column: "CurrentLetterId");

            migrationBuilder.CreateIndex(
                name: "IX_NonPlayerCharacters_GameId",
                schema: "LetterJam",
                table: "NonPlayerCharacters",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_CurrentLetterId",
                schema: "LetterJam",
                table: "PlayerStates",
                column: "CurrentLetterId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_GameId",
                schema: "LetterJam",
                table: "PlayerStates",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_PlayerId",
                schema: "LetterJam",
                table: "PlayerStates",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_GameId",
                schema: "LetterJam",
                table: "Rounds",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_LetterCards_Games_GameId",
                schema: "LetterJam",
                table: "LetterCards",
                column: "GameId",
                principalSchema: "LetterJam",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NonPlayerCharacters_Games_GameId",
                schema: "LetterJam",
                table: "NonPlayerCharacters",
                column: "GameId",
                principalSchema: "LetterJam",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_Games_GameId",
                schema: "LetterJam",
                table: "PlayerStates",
                column: "GameId",
                principalSchema: "LetterJam",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Games_GameId",
                schema: "LetterJam",
                table: "Rounds",
                column: "GameId",
                principalSchema: "LetterJam",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Rounds_CurrentRoundId",
                schema: "LetterJam",
                table: "Games");

            migrationBuilder.DropTable(
                name: "NonPlayerCharacters",
                schema: "LetterJam");

            migrationBuilder.DropTable(
                name: "PlayerStates",
                schema: "LetterJam");

            migrationBuilder.DropTable(
                name: "LetterCards",
                schema: "LetterJam");

            migrationBuilder.DropTable(
                name: "Rounds",
                schema: "LetterJam");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "LetterJam");
        }
    }
}
