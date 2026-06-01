using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class SurfaceLiquidState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SurfaceLiquidData",
                table: "GameItems",
                type: "mediumtext",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "SurfaceLiquidData",
                table: "Cells",
                type: "mediumtext",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "SurfaceLiquidData",
                table: "Bodies",
                type: "mediumtext",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SurfaceLiquidData",
                table: "GameItems");

            migrationBuilder.DropColumn(
                name: "SurfaceLiquidData",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "SurfaceLiquidData",
                table: "Bodies");
        }
    }
}
