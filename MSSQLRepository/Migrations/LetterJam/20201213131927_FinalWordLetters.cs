using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.LetterJam
{
    public partial class FinalWordLetters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinalWordLetters",
                schema: "LetterJam",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    CardId = table.Column<Guid>(nullable: true),
                    LetterIndex = table.Column<int>(nullable: false),
                    Letter = table.Column<string>(nullable: false),
                    PlayerLetterGuess = table.Column<string>(nullable: true),
                    BonusLetter = table.Column<bool>(nullable: false),
                    Wildcard = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalWordLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinalWordLetters_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "LetterJam",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinalWordLetters_GameId",
                schema: "LetterJam",
                table: "FinalWordLetters",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinalWordLetters",
                schema: "LetterJam");
        }
    }
}
