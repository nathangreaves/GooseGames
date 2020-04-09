using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations
{
    public partial class LinkFromResponseToOtherEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Responses_PlayerId",
                schema: "JustOne",
                table: "Responses",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_RoundId",
                schema: "JustOne",
                table: "Responses",
                column: "RoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Players_PlayerId",
                schema: "JustOne",
                table: "Responses",
                column: "PlayerId",
                principalSchema: "JustOne",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Rounds_RoundId",
                schema: "JustOne",
                table: "Responses",
                column: "RoundId",
                principalSchema: "JustOne",
                principalTable: "Rounds",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Players_PlayerId",
                schema: "JustOne",
                table: "Responses");

            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Rounds_RoundId",
                schema: "JustOne",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Responses_PlayerId",
                schema: "JustOne",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Responses_RoundId",
                schema: "JustOne",
                table: "Responses");
        }
    }
}
