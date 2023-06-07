using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewAssignments_ConferenceReviewers_ConferenceReviewerId",
                table: "ReviewAssignments");

            migrationBuilder.DropTable(
                name: "ConferenceReviewerSubjectAreas");

            migrationBuilder.DropTable(
                name: "ConferenceReviewers");

            migrationBuilder.DropColumn(
                name: "ConferenceAccountId",
                table: "ReviewAssignments");

            migrationBuilder.RenameColumn(
                name: "ConferenceReviewerId",
                table: "ReviewAssignments",
                newName: "ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewAssignments_ConferenceReviewerId",
                table: "ReviewAssignments",
                newName: "IX_ReviewAssignments_ReviewerId");

            migrationBuilder.AddColumn<int>(
                name: "Factor",
                table: "ConferenceRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quota = table.Column<int>(type: "int", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviewers_Incumbents_Id",
                        column: x => x.Id,
                        principalTable: "Incumbents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewerSubjectAreas",
                columns: table => new
                {
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewerSubjectAreas", x => new { x.ReviewerId, x.SubjectAreaId });
                    table.ForeignKey(
                        name: "FK_ReviewerSubjectAreas_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewerSubjectAreas_SubjectAreas_SubjectAreaId",
                        column: x => x.SubjectAreaId,
                        principalTable: "SubjectAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerSubjectAreas_SubjectAreaId",
                table: "ReviewerSubjectAreas",
                column: "SubjectAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewAssignments_Reviewers_ReviewerId",
                table: "ReviewAssignments",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewAssignments_Reviewers_ReviewerId",
                table: "ReviewAssignments");

            migrationBuilder.DropTable(
                name: "ReviewerSubjectAreas");

            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.DropColumn(
                name: "Factor",
                table: "ConferenceRoles");

            migrationBuilder.RenameColumn(
                name: "ReviewerId",
                table: "ReviewAssignments",
                newName: "ConferenceReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewAssignments_ReviewerId",
                table: "ReviewAssignments",
                newName: "IX_ReviewAssignments_ConferenceReviewerId");

            migrationBuilder.AddColumn<Guid>(
                name: "ConferenceAccountId",
                table: "ReviewAssignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ConferenceReviewers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quota = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConferenceReviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConferenceReviewers_ConferenceAccounts_Id",
                        column: x => x.Id,
                        principalTable: "ConferenceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConferenceReviewerSubjectAreas",
                columns: table => new
                {
                    ConferenceReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConferenceReviewerSubjectAreas", x => new { x.ConferenceReviewerId, x.SubjectAreaId });
                    table.ForeignKey(
                        name: "FK_ConferenceReviewerSubjectAreas_ConferenceReviewers_ConferenceReviewerId",
                        column: x => x.ConferenceReviewerId,
                        principalTable: "ConferenceReviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConferenceReviewerSubjectAreas_SubjectAreas_SubjectAreaId",
                        column: x => x.SubjectAreaId,
                        principalTable: "SubjectAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceReviewerSubjectAreas_SubjectAreaId",
                table: "ConferenceReviewerSubjectAreas",
                column: "SubjectAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewAssignments_ConferenceReviewers_ConferenceReviewerId",
                table: "ReviewAssignments",
                column: "ConferenceReviewerId",
                principalTable: "ConferenceReviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
