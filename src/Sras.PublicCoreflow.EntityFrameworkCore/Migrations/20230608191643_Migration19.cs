using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionSubjectAreas",
                table: "SubmissionSubjectAreas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewerSubjectAreas",
                table: "ReviewerSubjectAreas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conflicts",
                table: "Conflicts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "SubmissionSubjectAreas",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "SubmissionSubjectAreas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "ReviewerSubjectAreas",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ReviewerSubjectAreas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "Conflicts",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Conflicts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "Authors",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Authors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionSubjectAreas",
                table: "SubmissionSubjectAreas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewerSubjectAreas",
                table: "ReviewerSubjectAreas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conflicts",
                table: "Conflicts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionSubjectAreas_SubmissionId",
                table: "SubmissionSubjectAreas",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerSubjectAreas_ReviewerId",
                table: "ReviewerSubjectAreas",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_SubmissionId",
                table: "Conflicts",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_ParticipantId",
                table: "Authors",
                column: "ParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionSubjectAreas",
                table: "SubmissionSubjectAreas");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionSubjectAreas_SubmissionId",
                table: "SubmissionSubjectAreas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewerSubjectAreas",
                table: "ReviewerSubjectAreas");

            migrationBuilder.DropIndex(
                name: "IX_ReviewerSubjectAreas_ReviewerId",
                table: "ReviewerSubjectAreas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conflicts",
                table: "Conflicts");

            migrationBuilder.DropIndex(
                name: "IX_Conflicts_SubmissionId",
                table: "Conflicts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Authors_ParticipantId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SubmissionSubjectAreas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ReviewerSubjectAreas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Conflicts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Authors");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "SubmissionSubjectAreas",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "ReviewerSubjectAreas",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "Conflicts",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "Authors",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionSubjectAreas",
                table: "SubmissionSubjectAreas",
                columns: new[] { "SubmissionId", "SubjectAreaId", "ConcurrencyStamp" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewerSubjectAreas",
                table: "ReviewerSubjectAreas",
                columns: new[] { "ReviewerId", "SubjectAreaId", "ConcurrencyStamp" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conflicts",
                table: "Conflicts",
                columns: new[] { "SubmissionId", "IncumbentId", "ConflictCaseId", "ConcurrencyStamp" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                columns: new[] { "ParticipantId", "SubmissionId", "ConcurrencyStamp" });
        }
    }
}
