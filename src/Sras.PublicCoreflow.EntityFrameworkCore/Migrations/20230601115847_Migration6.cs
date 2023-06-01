using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Participants_ParticipantID",
                table: "AbpUsers");

            migrationBuilder.RenameColumn(
                name: "ParticipantID",
                table: "AbpUsers",
                newName: "ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_AbpUsers_ParticipantID",
                table: "AbpUsers",
                newName: "IX_AbpUsers_ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Participants_ParticipantId",
                table: "AbpUsers",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Participants_ParticipantId",
                table: "AbpUsers");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "AbpUsers",
                newName: "ParticipantID");

            migrationBuilder.RenameIndex(
                name: "IX_AbpUsers_ParticipantId",
                table: "AbpUsers",
                newName: "IX_AbpUsers_ParticipantID");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Participants_ParticipantID",
                table: "AbpUsers",
                column: "ParticipantID",
                principalTable: "Participants",
                principalColumn: "Id");
        }
    }
}
