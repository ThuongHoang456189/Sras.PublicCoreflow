using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConferenceId",
                table: "EmailTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TrackId",
                table: "EmailTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_ConferenceId",
                table: "EmailTemplates",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_TrackId",
                table: "EmailTemplates",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplates_Conferences_ConferenceId",
                table: "EmailTemplates",
                column: "ConferenceId",
                principalTable: "Conferences",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplates_Tracks_TrackId",
                table: "EmailTemplates",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplates_Conferences_ConferenceId",
                table: "EmailTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplates_Tracks_TrackId",
                table: "EmailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_ConferenceId",
                table: "EmailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_TrackId",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "ConferenceId",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "EmailTemplates");
        }
    }
}
