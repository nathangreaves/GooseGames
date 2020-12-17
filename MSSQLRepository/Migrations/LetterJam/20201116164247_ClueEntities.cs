using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.LetterJam
{
    public partial class ClueEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clues",
                schema: "LetterJam",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoundId = table.Column<Guid>(nullable: false),
                    RoundNumber = table.Column<int>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    ClueGiverPlayerId = table.Column<Guid>(nullable: false),
                    ClueSuccessful = table.Column<bool>(nullable: false),
                    NumberOfLetters = table.Column<int>(nullable: false),
                    NumberOfPlayerLetters = table.Column<int>(nullable: false),
                    NumberOfNonPlayerLetters = table.Column<int>(nullable: false),
                    WildcardUsed = table.Column<bool>(nullable: false),
                    NumberOfBonusLetters = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clues_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalSchema: "LetterJam",
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClueLetters",
                schema: "LetterJam",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClueId = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: true),
                    NonPlayerCharacterId = table.Column<Guid>(nullable: true),
                    LetterId = table.Column<Guid>(nullable: true),
                    Letter = table.Column<string>(nullable: true),
                    BonusLetter = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClueLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClueLetters_Clues_ClueId",
                        column: x => x.ClueId,
                        principalSchema: "LetterJam",
                        principalTable: "Clues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClueVotes",
                schema: "LetterJam",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    ClueId = table.Column<Guid>(nullable: false),
                    RoundId = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClueVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClueVotes_Clues_ClueId",
                        column: x => x.ClueId,
                        principalSchema: "LetterJam",
                        principalTable: "Clues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClueVotes_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalSchema: "LetterJam",
                        principalTable: "Rounds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClueLetters_ClueId",
                schema: "LetterJam",
                table: "ClueLetters",
                column: "ClueId");

            migrationBuilder.CreateIndex(
                name: "IX_Clues_RoundId",
                schema: "LetterJam",
                table: "Clues",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_ClueVotes_ClueId",
                schema: "LetterJam",
                table: "ClueVotes",
                column: "ClueId");

            migrationBuilder.CreateIndex(
                name: "IX_ClueVotes_RoundId",
                schema: "LetterJam",
                table: "ClueVotes",
                column: "RoundId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClueLetters",
                schema: "LetterJam");

            migrationBuilder.DropTable(
                name: "ClueVotes",
                schema: "LetterJam");

            migrationBuilder.DropTable(
                name: "Clues",
                schema: "LetterJam");
        }
    }
}
