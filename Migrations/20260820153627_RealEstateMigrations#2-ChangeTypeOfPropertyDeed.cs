using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations;

#pragma warning disable CA1515
/// <inheritdoc />
public partial class RealEstateMigrations2ChangeTypeOfPropertyDeed : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.AlterColumn<long>(
            name: "RegistryNumber",
            table: "PropertyDeeds",
            type: "bigint",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<long>(
            name: "DeedNumber",
            table: "PropertyDeeds",
            type: "bigint",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.AlterColumn<string>(
            name: "RegistryNumber",
            table: "PropertyDeeds",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.AlterColumn<string>(
            name: "DeedNumber",
            table: "PropertyDeeds",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(long),
            oldType: "bigint");
    }
}
