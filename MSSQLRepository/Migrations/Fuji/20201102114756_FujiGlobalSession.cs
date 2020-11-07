using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Fuji
{
    public partial class FujiGlobalSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_Sessions_SessionId",
                schema: "Fuji",
                table: "DeckCards");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscardedCards_Sessions_SessionId",
                schema: "Fuji",
                table: "DiscardedCards");

            migrationBuilder.DropForeignKey(
                name: "FK_HandCards_Players_PlayerId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropForeignKey(
                name: "FK_HandCards_Sessions_SessionId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "Fuji",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "Fuji");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "Fuji");

            migrationBuilder.DropIndex(
                name: "IX_HandCards_PlayerId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropIndex(
                name: "IX_HandCards_SessionId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropIndex(
                name: "IX_DiscardedCards_SessionId",
                schema: "Fuji",
                table: "DiscardedCards");

            migrationBuilder.DropIndex(
                name: "IX_DeckCards_SessionId",
                schema: "Fuji",
                table: "DeckCards");

            migrationBuilder.DropColumn(
                name: "SessionId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropColumn(
                name: "SessionId",
                schema: "Fuji",
                table: "DiscardedCards");

            migrationBuilder.DropColumn(
                name: "SessionId",
                schema: "Fuji",
                table: "DeckCards");

            migrationBuilder.Sql("DELETE FROM Fuji.HandCards", false);
            migrationBuilder.Sql("DELETE FROM Fuji.DeckCards", false);
            migrationBuilder.Sql("DELETE FROM Fuji.DiscardedCards", false);

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                schema: "Fuji",
                table: "HandCards",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerInformationId",
                schema: "Fuji",
                table: "HandCards",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                schema: "Fuji",
                table: "DiscardedCards",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                schema: "Fuji",
                table: "DeckCards",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "Fuji",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    ActivePlayerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerInformation",
                schema: "Fuji",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    PlayedCardId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerInformation_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "Fuji",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerInformation_HandCards_PlayedCardId",
                        column: x => x.PlayedCardId,
                        principalSchema: "Fuji",
                        principalTable: "HandCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HandCards_GameId",
                schema: "Fuji",
                table: "HandCards",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_HandCards_PlayerInformationId",
                schema: "Fuji",
                table: "HandCards",
                column: "PlayerInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscardedCards_GameId",
                schema: "Fuji",
                table: "DiscardedCards",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_GameId",
                schema: "Fuji",
                table: "DeckCards",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerInformation_GameId",
                schema: "Fuji",
                table: "PlayerInformation",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerInformation_PlayedCardId",
                schema: "Fuji",
                table: "PlayerInformation",
                column: "PlayedCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_Games_GameId",
                schema: "Fuji",
                table: "DeckCards",
                column: "GameId",
                principalSchema: "Fuji",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscardedCards_Games_GameId",
                schema: "Fuji",
                table: "DiscardedCards",
                column: "GameId",
                principalSchema: "Fuji",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HandCards_Games_GameId",
                schema: "Fuji",
                table: "HandCards",
                column: "GameId",
                principalSchema: "Fuji",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HandCards_PlayerInformation_PlayerInformationId",
                schema: "Fuji",
                table: "HandCards",
                column: "PlayerInformationId",
                principalSchema: "Fuji",
                principalTable: "PlayerInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_Games_GameId",
                schema: "Fuji",
                table: "DeckCards");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscardedCards_Games_GameId",
                schema: "Fuji",
                table: "DiscardedCards");

            migrationBuilder.DropForeignKey(
                name: "FK_HandCards_Games_GameId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropForeignKey(
                name: "FK_HandCards_PlayerInformation_PlayerInformationId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropTable(
                name: "PlayerInformation",
                schema: "Fuji");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "Fuji");

            migrationBuilder.DropIndex(
                name: "IX_HandCards_GameId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropIndex(
                name: "IX_HandCards_PlayerInformationId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropIndex(
                name: "IX_DiscardedCards_GameId",
                schema: "Fuji",
                table: "DiscardedCards");

            migrationBuilder.DropIndex(
                name: "IX_DeckCards_GameId",
                schema: "Fuji",
                table: "DeckCards");

            migrationBuilder.DropColumn(
                name: "GameId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropColumn(
                name: "PlayerInformationId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropColumn(
                name: "GameId",
                schema: "Fuji",
                table: "DiscardedCards");

            migrationBuilder.DropColumn(
                name: "GameId",
                schema: "Fuji",
                table: "DeckCards");

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                schema: "Fuji",
                table: "HandCards",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                schema: "Fuji",
                table: "DiscardedCards",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                schema: "Fuji",
                table: "DeckCards",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "Fuji",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivePlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "Fuji",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayedCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlayerNumber = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_HandCards_PlayedCardId",
                        column: x => x.PlayedCardId,
                        principalSchema: "Fuji",
                        principalTable: "HandCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Players_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "Fuji",
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HandCards_PlayerId",
                schema: "Fuji",
                table: "HandCards",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_HandCards_SessionId",
                schema: "Fuji",
                table: "HandCards",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscardedCards_SessionId",
                schema: "Fuji",
                table: "DiscardedCards",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_SessionId",
                schema: "Fuji",
                table: "DeckCards",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayedCardId",
                schema: "Fuji",
                table: "Players",
                column: "PlayedCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_SessionId",
                schema: "Fuji",
                table: "Players",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ActivePlayerId",
                schema: "Fuji",
                table: "Sessions",
                column: "ActivePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionMasterId",
                schema: "Fuji",
                table: "Sessions",
                column: "SessionMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_Sessions_SessionId",
                schema: "Fuji",
                table: "DeckCards",
                column: "SessionId",
                principalSchema: "Fuji",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscardedCards_Sessions_SessionId",
                schema: "Fuji",
                table: "DiscardedCards",
                column: "SessionId",
                principalSchema: "Fuji",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HandCards_Players_PlayerId",
                schema: "Fuji",
                table: "HandCards",
                column: "PlayerId",
                principalSchema: "Fuji",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HandCards_Sessions_SessionId",
                schema: "Fuji",
                table: "HandCards",
                column: "SessionId",
                principalSchema: "Fuji",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Players_ActivePlayerId",
                schema: "Fuji",
                table: "Sessions",
                column: "ActivePlayerId",
                principalSchema: "Fuji",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Players_SessionMasterId",
                schema: "Fuji",
                table: "Sessions",
                column: "SessionMasterId",
                principalSchema: "Fuji",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
