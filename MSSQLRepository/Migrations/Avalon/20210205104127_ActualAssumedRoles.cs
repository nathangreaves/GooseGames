using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSQLRepository.Migrations.Avalon
{
    public partial class ActualAssumedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_GameRoles_GameRoleId",
                schema: "Avalon",
                table: "PlayerStates");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_GameRoleId",
                schema: "Avalon",
                table: "PlayerStates");

            migrationBuilder.AddColumn<Guid>(
                name: "ActualRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AssumedRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("update Avalon.PlayerStates set ActualRoleId = GameRoleId, AssumedRoleId = GameRoleId where 1 = 1");

            migrationBuilder.DropColumn(
                name: "GameRoleId",
                schema: "Avalon",
                table: "PlayerStates");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_ActualRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                column: "ActualRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_AssumedRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                column: "AssumedRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_GameRoles_ActualRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                column: "ActualRoleId",
                principalSchema: "Avalon",
                principalTable: "GameRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_GameRoles_AssumedRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                column: "AssumedRoleId",
                principalSchema: "Avalon",
                principalTable: "GameRoles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_GameRoles_ActualRoleId",
                schema: "Avalon",
                table: "PlayerStates");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_GameRoles_AssumedRoleId",
                schema: "Avalon",
                table: "PlayerStates");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_ActualRoleId",
                schema: "Avalon",
                table: "PlayerStates");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_AssumedRoleId",
                schema: "Avalon",
                table: "PlayerStates");

            migrationBuilder.AddColumn<Guid>(
                name: "GameRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("update Avalon.PlayerStates set GameRoleId = ActualRoleId where 1 = 1");

            migrationBuilder.DropColumn(
                name: "ActualRoleId",
                schema: "Avalon",
                table: "PlayerStates");

            migrationBuilder.DropColumn(
                name: "AssumedRoleId",
                schema: "Avalon",
                table: "PlayerStates");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_GameRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                column: "GameRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_GameRoles_GameRoleId",
                schema: "Avalon",
                table: "PlayerStates",
                column: "GameRoleId",
                principalSchema: "Avalon",
                principalTable: "GameRoles",
                principalColumn: "Id");
        }
    }
}
