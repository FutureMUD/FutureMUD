using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class FirearmAttachmentsAndProjectileAmmunition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectileCount",
                table: "AmmunitionTypes",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'1'");

            migrationBuilder.AddColumn<int>(
                name: "ScatterType",
                table: "AmmunitionTypes",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SpreadPenalty",
                table: "AmmunitionTypes",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectileCount",
                table: "AmmunitionTypes");

            migrationBuilder.DropColumn(
                name: "ScatterType",
                table: "AmmunitionTypes");

            migrationBuilder.DropColumn(
                name: "SpreadPenalty",
                table: "AmmunitionTypes");
        }
    }
}
