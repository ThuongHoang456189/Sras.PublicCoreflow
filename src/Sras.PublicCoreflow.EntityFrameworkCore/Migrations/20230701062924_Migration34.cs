using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_QuestionGroupTracks_QuestionGroupTrackId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Questions_NextQuestionId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "QuestionGroupTracks");

            migrationBuilder.DropIndex(
                name: "IX_Questions_NextQuestionId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "NextQuestionId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Settings",
                table: "QuestionGroups");

            migrationBuilder.RenameColumn(
                name: "Settings",
                table: "Questions",
                newName: "ShowAs");

            migrationBuilder.RenameColumn(
                name: "QuestionGroupTrackId",
                table: "Questions",
                newName: "TrackId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_QuestionGroupTrackId",
                table: "Questions",
                newName: "IX_Questions_TrackId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Questions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Questions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "Questions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisibleToReviewers",
                table: "Questions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionGroupId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Questions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                table: "Questions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "QuestionGroups",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionGroupId",
                table: "Questions",
                column: "QuestionGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_QuestionGroups_QuestionGroupId",
                table: "Questions",
                column: "QuestionGroupId",
                principalTable: "QuestionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Tracks_TrackId",
                table: "Questions",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_QuestionGroups_QuestionGroupId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Tracks_TrackId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_QuestionGroupId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IsVisibleToReviewers",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionGroupId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "Questions",
                newName: "QuestionGroupTrackId");

            migrationBuilder.RenameColumn(
                name: "ShowAs",
                table: "Questions",
                newName: "Settings");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_TrackId",
                table: "Questions",
                newName: "IX_Questions_QuestionGroupTrackId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NextQuestionId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "QuestionGroups",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<string>(
                name: "Settings",
                table: "QuestionGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "QuestionGroupTracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionGroupTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionGroupTracks_QuestionGroups_QuestionGroupId",
                        column: x => x.QuestionGroupId,
                        principalTable: "QuestionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionGroupTracks_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_NextQuestionId",
                table: "Questions",
                column: "NextQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionGroupTracks_QuestionGroupId",
                table: "QuestionGroupTracks",
                column: "QuestionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionGroupTracks_TrackId",
                table: "QuestionGroupTracks",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_QuestionGroupTracks_QuestionGroupTrackId",
                table: "Questions",
                column: "QuestionGroupTrackId",
                principalTable: "QuestionGroupTracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Questions_NextQuestionId",
                table: "Questions",
                column: "NextQuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }
    }
}
