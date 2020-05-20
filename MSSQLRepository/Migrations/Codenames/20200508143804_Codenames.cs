using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Codenames
{
    public partial class Codenames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Codenames");

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "Codenames",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    IsBlueTurn = table.Column<bool>(nullable: false),
                    BlueVictory = table.Column<bool>(nullable: true),
                    SessionWordsId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                schema: "Codenames",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    SessionWordsId = table.Column<Guid>(nullable: false),
                    Word = table.Column<string>(nullable: true),
                    WordType = table.Column<int>(nullable: false),
                    Revealed = table.Column<bool>(nullable: false),
                    RevealedByBlue = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Words_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "Codenames",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Words_SessionId",
                schema: "Codenames",
                table: "Words",
                column: "SessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Words",
                schema: "Codenames");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "Codenames");
        }
    }
}
