using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherCertificates",
                table: "ResearcherProfiles",
                newName: "Skills");

            migrationBuilder.RenameColumn(
                name: "Languages",
                table: "ResearcherProfiles",
                newName: "ORCID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Skills",
                table: "ResearcherProfiles",
                newName: "OtherCertificates");

            migrationBuilder.RenameColumn(
                name: "ORCID",
                table: "ResearcherProfiles",
                newName: "Languages");
        }
    }
}
