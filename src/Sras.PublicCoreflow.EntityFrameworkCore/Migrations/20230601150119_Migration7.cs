using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAccepted",
                table: "Conferences",
                newName: "IsSingleTrack");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSingleTrack",
                table: "Conferences",
                newName: "IsAccepted");
        }
    }
}
