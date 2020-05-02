using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Fuji
{
    public partial class FujiInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Fuji");

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "Fuji",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    PlayerNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ConnectionId = table.Column<string>(nullable: true),
                    PlayedCardId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "Fuji",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    SessionMasterId = table.Column<Guid>(nullable: true),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Players_SessionMasterId",
                        column: x => x.SessionMasterId,
                        principalSchema: "Fuji",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeckCards",
                schema: "Fuji",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    FaceValue = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeckCards_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "Fuji",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscardedCards",
                schema: "Fuji",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    FaceValue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscardedCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscardedCards_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "Fuji",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HandCards",
                schema: "Fuji",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    FaceValue = table.Column<int>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandCards_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "Fuji",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HandCards_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "Fuji",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_SessionId",
                schema: "Fuji",
                table: "DeckCards",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscardedCards_SessionId",
                schema: "Fuji",
                table: "DiscardedCards",
                column: "SessionId");

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
                name: "IX_Sessions_SessionMasterId",
                schema: "Fuji",
                table: "Sessions",
                column: "SessionMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "Fuji",
                table: "Players",
                column: "SessionId",
                principalSchema: "Fuji",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_HandCards_PlayedCardId",
                schema: "Fuji",
                table: "Players",
                column: "PlayedCardId",
                principalSchema: "Fuji",
                principalTable: "HandCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandCards_Sessions_SessionId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "Fuji",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_HandCards_Players_PlayerId",
                schema: "Fuji",
                table: "HandCards");

            migrationBuilder.DropTable(
                name: "DeckCards",
                schema: "Fuji");

            migrationBuilder.DropTable(
                name: "DiscardedCards",
                schema: "Fuji");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "Fuji");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "Fuji");

            migrationBuilder.DropTable(
                name: "HandCards",
                schema: "Fuji");
        }
    }
}
