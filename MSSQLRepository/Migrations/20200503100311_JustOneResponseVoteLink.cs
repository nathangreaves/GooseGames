using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations
{
    public partial class JustOneResponseVoteLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseVotes_Responses_ResponseId",
                schema: "JustOne",
                table: "ResponseVotes");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseVotes_Responses_ResponseId",
                schema: "JustOne",
                table: "ResponseVotes",
                column: "ResponseId",
                principalSchema: "JustOne",
                principalTable: "Responses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseVotes_Responses_ResponseId",
                schema: "JustOne",
                table: "ResponseVotes");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseVotes_Responses_ResponseId",
                schema: "JustOne",
                table: "ResponseVotes",
                column: "ResponseId",
                principalSchema: "JustOne",
                principalTable: "Responses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
