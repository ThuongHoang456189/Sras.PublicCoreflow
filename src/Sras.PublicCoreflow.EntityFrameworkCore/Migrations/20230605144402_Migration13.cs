using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Incumbents_CreatedIncumbentId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Incumbents_LastModifiedIncumbentId",
                table: "Submissions");

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedIncumbentId",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedIncumbentId",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Incumbents_CreatedIncumbentId",
                table: "Submissions",
                column: "CreatedIncumbentId",
                principalTable: "Incumbents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Incumbents_LastModifiedIncumbentId",
                table: "Submissions",
                column: "LastModifiedIncumbentId",
                principalTable: "Incumbents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Incumbents_CreatedIncumbentId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Incumbents_LastModifiedIncumbentId",
                table: "Submissions");

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedIncumbentId",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedIncumbentId",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Incumbents_CreatedIncumbentId",
                table: "Submissions",
                column: "CreatedIncumbentId",
                principalTable: "Incumbents",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Incumbents_LastModifiedIncumbentId",
                table: "Submissions",
                column: "LastModifiedIncumbentId",
                principalTable: "Incumbents",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
