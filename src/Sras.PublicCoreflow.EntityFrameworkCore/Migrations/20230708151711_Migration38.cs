using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_AbpUsers_Id",
                table: "Profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles");

            migrationBuilder.RenameTable(
                name: "Profiles",
                newName: "ResearcherProfiles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResearcherProfiles",
                table: "ResearcherProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResearcherProfiles_AbpUsers_Id",
                table: "ResearcherProfiles",
                column: "Id",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResearcherProfiles_AbpUsers_Id",
                table: "ResearcherProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResearcherProfiles",
                table: "ResearcherProfiles");

            migrationBuilder.RenameTable(
                name: "ResearcherProfiles",
                newName: "Profiles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_AbpUsers_Id",
                table: "Profiles",
                column: "Id",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
