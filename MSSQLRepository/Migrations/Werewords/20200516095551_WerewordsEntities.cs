using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Werewords
{
    public partial class WerewordsEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Werewords");

            migrationBuilder.CreateTable(
                name: "PlayerRoundInformation",
                schema: "Werewords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    RoundId = table.Column<Guid>(nullable: false),
                    SecretRole = table.Column<int>(nullable: false),
                    Ticks = table.Column<int>(nullable: false),
                    Crosses = table.Column<int>(nullable: false),
                    QuestionMarks = table.Column<int>(nullable: false),
                    SoClose = table.Column<int>(nullable: false),
                    Correct = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRoundInformation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerVotes",
                schema: "Werewords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    RoundId = table.Column<Guid>(nullable: false),
                    VotedPlayerId = table.Column<Guid>(nullable: false),
                    VoteType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerVotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                schema: "Werewords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    MayorId = table.Column<Guid>(nullable: true),
                    SecretWord = table.Column<string>(nullable: true),
                    RoundStartedUtc = table.Column<DateTime>(nullable: false),
                    RoundDurationMinutes = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Outcome = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "Werewords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    SessionMasterId = table.Column<Guid>(nullable: true),
                    CurrentRoundId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Rounds_CurrentRoundId",
                        column: x => x.CurrentRoundId,
                        principalSchema: "Werewords",
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "Werewords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    PlayerNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ConnectionId = table.Column<string>(nullable: true),
                    Status = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "Werewords",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRoundInformation_PlayerId",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRoundInformation_RoundId",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_SessionId",
                schema: "Werewords",
                table: "Players",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerVotes_PlayerId",
                schema: "Werewords",
                table: "PlayerVotes",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerVotes_RoundId",
                schema: "Werewords",
                table: "PlayerVotes",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerVotes_VotedPlayerId",
                schema: "Werewords",
                table: "PlayerVotes",
                column: "VotedPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_MayorId",
                schema: "Werewords",
                table: "Rounds",
                column: "MayorId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_SessionId",
                schema: "Werewords",
                table: "Rounds",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CurrentRoundId",
                schema: "Werewords",
                table: "Sessions",
                column: "CurrentRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionMasterId",
                schema: "Werewords",
                table: "Sessions",
                column: "SessionMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerRoundInformation_Players_PlayerId",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                column: "PlayerId",
                principalSchema: "Werewords",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerRoundInformation_Rounds_RoundId",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                column: "RoundId",
                principalSchema: "Werewords",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerVotes_Players_PlayerId",
                schema: "Werewords",
                table: "PlayerVotes",
                column: "PlayerId",
                principalSchema: "Werewords",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerVotes_Players_VotedPlayerId",
                schema: "Werewords",
                table: "PlayerVotes",
                column: "VotedPlayerId",
                principalSchema: "Werewords",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerVotes_Rounds_RoundId",
                schema: "Werewords",
                table: "PlayerVotes",
                column: "RoundId",
                principalSchema: "Werewords",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Players_MayorId",
                schema: "Werewords",
                table: "Rounds",
                column: "MayorId",
                principalSchema: "Werewords",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Sessions_SessionId",
                schema: "Werewords",
                table: "Rounds",
                column: "SessionId",
                principalSchema: "Werewords",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Players_SessionMasterId",
                schema: "Werewords",
                table: "Sessions",
                column: "SessionMasterId",
                principalSchema: "Werewords",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Players_MayorId",
                schema: "Werewords",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Players_SessionMasterId",
                schema: "Werewords",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Rounds_CurrentRoundId",
                schema: "Werewords",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "PlayerRoundInformation",
                schema: "Werewords");

            migrationBuilder.DropTable(
                name: "PlayerVotes",
                schema: "Werewords");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "Werewords");

            migrationBuilder.DropTable(
                name: "Rounds",
                schema: "Werewords");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "Werewords");
        }
    }
}
