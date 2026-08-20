using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations;

#pragma warning disable CA1515
/// <inheritdoc />
public partial class _20260820153627_RealEstateMigrations3ChangeRelationAtPropertDeedAndPropertyDeedImageFrom1to1To1ToMany : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropIndex(
            name: "IX_PropertyDeedImages_PropertyDeedId",
            table: "PropertyDeedImages");

        migrationBuilder.CreateIndex(
            name: "IX_PropertyDeedImages_PropertyDeedId",
            table: "PropertyDeedImages",
            column: "PropertyDeedId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropIndex(
            name: "IX_PropertyDeedImages_PropertyDeedId",
            table: "PropertyDeedImages");

        migrationBuilder.CreateIndex(
            name: "IX_PropertyDeedImages_PropertyDeedId",
            table: "PropertyDeedImages",
            column: "PropertyDeedId",
            unique: true);
    }
}
