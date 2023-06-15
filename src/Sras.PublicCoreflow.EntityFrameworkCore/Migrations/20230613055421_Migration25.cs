using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimaryContact",
                table: "Incumbents");

            migrationBuilder.AddColumn<bool>(
                name: "IsDecisionMaker",
                table: "Incumbents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDecisionMaker",
                table: "Incumbents");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimaryContact",
                table: "Incumbents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
