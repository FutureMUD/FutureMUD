using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class NaturalRangedAttacksAndElementalContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OnUseProgId",
                table: "WeaponAttacks",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SurfaceReactionInfo",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<double>(
                name: "OxidationFactor",
                table: "Gases",
                type: "double",
                nullable: false,
                defaultValueSql: "'1.0'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnUseProgId",
                table: "WeaponAttacks");

            migrationBuilder.DropColumn(
                name: "SurfaceReactionInfo",
                table: "Liquids");

            migrationBuilder.DropColumn(
                name: "OxidationFactor",
                table: "Gases");
        }
    }
}
