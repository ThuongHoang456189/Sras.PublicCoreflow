using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportedPlaceholders_EmailTemplates_EmailTemplateId",
                table: "SupportedPlaceholders");

            migrationBuilder.DropIndex(
                name: "IX_SupportedPlaceholders_EmailTemplateId",
                table: "SupportedPlaceholders");

            migrationBuilder.DropColumn(
                name: "EmailTemplateId",
                table: "SupportedPlaceholders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmailTemplateId",
                table: "SupportedPlaceholders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SupportedPlaceholders_EmailTemplateId",
                table: "SupportedPlaceholders",
                column: "EmailTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportedPlaceholders_EmailTemplates_EmailTemplateId",
                table: "SupportedPlaceholders",
                column: "EmailTemplateId",
                principalTable: "EmailTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
