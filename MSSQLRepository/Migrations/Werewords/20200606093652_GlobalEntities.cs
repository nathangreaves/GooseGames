using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Werewords
{
    public partial class GlobalEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerRoundInformation_Players_PlayerId",
                schema: "Werewords",
                table: "PlayerRoundInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerVotes_Players_PlayerId",
                schema: "Werewords",
                table: "PlayerVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerVotes_Players_VotedPlayerId",
                schema: "Werewords",
                table: "PlayerVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Players_MayorId",
                schema: "Werewords",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Sessions_SessionId",
                schema: "Werewords",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Sessions_SessionId",
                schema: "Werewords",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "Werewords");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "Werewords");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_MayorId",
                schema: "Werewords",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_SessionId",
                schema: "Werewords",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_PlayerVotes_PlayerId",
                schema: "Werewords",
                table: "PlayerVotes");

            migrationBuilder.DropIndex(
                name: "IX_PlayerVotes_VotedPlayerId",
                schema: "Werewords",
                table: "PlayerVotes");

            migrationBuilder.DropIndex(
                name: "IX_PlayerRoundInformation_PlayerId",
                schema: "Werewords",
                table: "PlayerRoundInformation");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Status",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Werewords",
                table: "PlayerRoundInformation");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Werewords",
                table: "PlayerRoundInformation");

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "Werewords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentRoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SessionMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerNumber = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_PlayerVotes_PlayerId",
                schema: "Werewords",
                table: "PlayerVotes",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerVotes_VotedPlayerId",
                schema: "Werewords",
                table: "PlayerVotes",
                column: "VotedPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRoundInformation_PlayerId",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_SessionId",
                schema: "Werewords",
                table: "Players",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CurrentRoundId",
                schema: "Werewords",
                table: "Sessions",
                column: "CurrentRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Password",
                schema: "Werewords",
                table: "Sessions",
                column: "Password",
                unique: true,
                filter: "[Password] IS NOT NULL");

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
    }
}
