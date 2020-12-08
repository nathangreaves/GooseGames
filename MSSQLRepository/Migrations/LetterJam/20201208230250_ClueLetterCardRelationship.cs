using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.LetterJam
{
    public partial class ClueLetterCardRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LetterCardId",
                schema: "LetterJam",
                table: "ClueLetters",
                nullable: true);

            migrationBuilder.Sql("Update LetterJam.ClueLetters set LetterCardId = LetterId where 1 = 1;");

            migrationBuilder.DropColumn(
                name: "LetterId",
                schema: "LetterJam",
                table: "ClueLetters");

            migrationBuilder.CreateIndex(
                name: "IX_ClueLetters_LetterCardId",
                schema: "LetterJam",
                table: "ClueLetters",
                column: "LetterCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClueLetters_LetterCards_LetterCardId",
                schema: "LetterJam",
                table: "ClueLetters",
                column: "LetterCardId",
                principalSchema: "LetterJam",
                principalTable: "LetterCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClueLetters_LetterCards_LetterCardId",
                schema: "LetterJam",
                table: "ClueLetters");

            migrationBuilder.DropIndex(
                name: "IX_ClueLetters_LetterCardId",
                schema: "LetterJam",
                table: "ClueLetters");

            migrationBuilder.AddColumn<Guid>(
                name: "LetterId",
                schema: "LetterJam",
                table: "ClueLetters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("Update LetterJam.ClueLetters set LetterId = LetterCardId where 1 = 1;");

            migrationBuilder.DropColumn(
                name: "LetterCardId",
                schema: "LetterJam",
                table: "ClueLetters");

        }
    }
}
