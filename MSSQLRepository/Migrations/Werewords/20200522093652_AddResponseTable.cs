using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Werewords
{
    public partial class AddResponseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Correct",
                schema: "Werewords",
                table: "PlayerRoundInformation");

            migrationBuilder.DropColumn(
                name: "Crosses",
                schema: "Werewords",
                table: "PlayerRoundInformation");

            migrationBuilder.DropColumn(
                name: "QuestionMarks",
                schema: "Werewords",
                table: "PlayerRoundInformation");

            migrationBuilder.DropColumn(
                name: "SoClose",
                schema: "Werewords",
                table: "PlayerRoundInformation");

            migrationBuilder.DropColumn(
                name: "Ticks",
                schema: "Werewords",
                table: "PlayerRoundInformation");

            migrationBuilder.CreateTable(
                name: "PlayerResponses",
                schema: "Werewords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    PlayerRoundInformationId = table.Column<Guid>(nullable: false),
                    ResponseType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerResponses_PlayerRoundInformation_PlayerRoundInformationId",
                        column: x => x.PlayerRoundInformationId,
                        principalSchema: "Werewords",
                        principalTable: "PlayerRoundInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerResponses_PlayerRoundInformationId",
                schema: "Werewords",
                table: "PlayerResponses",
                column: "PlayerRoundInformationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerResponses",
                schema: "Werewords");

            migrationBuilder.AddColumn<int>(
                name: "Correct",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Crosses",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionMarks",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SoClose",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Ticks",
                schema: "Werewords",
                table: "PlayerRoundInformation",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
