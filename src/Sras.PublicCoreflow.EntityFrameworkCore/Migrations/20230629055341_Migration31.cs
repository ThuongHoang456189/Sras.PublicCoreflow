using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NavBar",
                table: "WebTemplates",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecisionChecklist",
                table: "Tracks",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentationSettings",
                table: "Tracks",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionSettings",
                table: "Tracks",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NamePrefix",
                table: "Outsiders",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Answers",
                table: "CameraReadies",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanSkip",
                table: "ActivityDeadlines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionTime",
                table: "ActivityDeadlines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Factor",
                table: "ActivityDeadlines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GuidelineGroup",
                table: "ActivityDeadlines",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBeginPhaseMark",
                table: "ActivityDeadlines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "ActivityDeadlines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGuidelineShowed",
                table: "ActivityDeadlines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNext",
                table: "ActivityDeadlines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phase",
                table: "ActivityDeadlines",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanDeadline",
                table: "ActivityDeadlines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NamePrefix",
                table: "AbpUsers",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Guidelines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    GuidelineGroup = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsChairOnly = table.Column<bool>(type: "bit", nullable: false),
                    Route = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Factor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guidelines", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guidelines");

            migrationBuilder.DropColumn(
                name: "NavBar",
                table: "WebTemplates");

            migrationBuilder.DropColumn(
                name: "DecisionChecklist",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "PresentationSettings",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "RevisionSettings",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "NamePrefix",
                table: "Outsiders");

            migrationBuilder.DropColumn(
                name: "Answers",
                table: "CameraReadies");

            migrationBuilder.DropColumn(
                name: "CanSkip",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "CompletionTime",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "Factor",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "GuidelineGroup",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "IsBeginPhaseMark",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "IsGuidelineShowed",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "IsNext",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "Phase",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "PlanDeadline",
                table: "ActivityDeadlines");

            migrationBuilder.DropColumn(
                name: "NamePrefix",
                table: "AbpUsers");
        }
    }
}
