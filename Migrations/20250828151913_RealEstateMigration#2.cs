using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations
{
    /// <inheritdoc />
    public partial class RealEstateMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Assets",
                newName: "ContractType");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Assets",
                newName: "AssetType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContractType",
                table: "Assets",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "AssetType",
                table: "Assets",
                newName: "Name");
        }
    }
}
