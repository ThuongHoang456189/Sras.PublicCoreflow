using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublishName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PrimaryEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    WebsiteAndSocialLinks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherIDs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlsoKnownAs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Introduction = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CurrentResearchScientistTitle = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CurrentAdministrationPosition = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CurrentAcademicFunction = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    YearOfCurrentAcademicFunctionAchievement = table.Column<int>(type: "int", nullable: true),
                    CurrentDegree = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    YearOfCurrentCurrentDegreeAchievement = table.Column<int>(type: "int", nullable: true),
                    HomeAddress = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    MobilePhoneNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Workplace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Educations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Employments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scholarships = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Awards = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Languages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherCertificates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResearchDirections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Publications = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_AbpUsers_Id",
                        column: x => x.Id,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
