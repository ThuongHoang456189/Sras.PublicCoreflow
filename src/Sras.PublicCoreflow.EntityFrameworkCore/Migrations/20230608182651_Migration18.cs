using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration18 : Migration
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "ReviewerSubjectAreas",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "Conflicts",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "Authors",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionSubjectAreas",
                table: "SubmissionSubjectAreas",
                columns: new[] { "SubmissionId", "SubjectAreaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewerSubjectAreas",
                table: "ReviewerSubjectAreas",
                columns: new[] { "ReviewerId", "SubjectAreaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conflicts",
                table: "Conflicts",
                columns: new[] { "SubmissionId", "IncumbentId", "ConflictCaseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                columns: new[] { "ParticipantId", "SubmissionId" });
        }
    }
}
