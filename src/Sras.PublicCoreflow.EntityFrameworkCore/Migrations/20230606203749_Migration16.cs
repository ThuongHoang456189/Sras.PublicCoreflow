using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Participants_ParticipantId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Outsiders_Participants_ParticipantId",
                table: "Outsiders");

            migrationBuilder.DropIndex(
                name: "IX_Outsiders_ParticipantId",
                table: "Outsiders");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_ParticipantId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "Outsiders");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "AbpUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Participants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OutsiderId",
                table: "Participants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_AccountId",
                table: "Participants",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_OutsiderId",
                table: "Participants",
                column: "OutsiderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_AbpUsers_AccountId",
                table: "Participants",
                column: "AccountId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Outsiders_OutsiderId",
                table: "Participants",
                column: "OutsiderId",
                principalTable: "Outsiders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_AbpUsers_AccountId",
                table: "Participants");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Outsiders_OutsiderId",
                table: "Participants");

            migrationBuilder.DropIndex(
                name: "IX_Participants_AccountId",
                table: "Participants");

            migrationBuilder.DropIndex(
                name: "IX_Participants_OutsiderId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "OutsiderId",
                table: "Participants");

            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "Outsiders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "AbpUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Outsiders_ParticipantId",
                table: "Outsiders",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_ParticipantId",
                table: "AbpUsers",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Participants_ParticipantId",
                table: "AbpUsers",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Outsiders_Participants_ParticipantId",
                table: "Outsiders",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
