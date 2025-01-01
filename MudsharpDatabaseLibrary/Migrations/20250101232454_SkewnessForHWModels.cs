using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class SkewnessForHWModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SkewnessBMI",
                table: "HeightWeightModels",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SkewnessHeight",
                table: "HeightWeightModels",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SkewnessWeight",
                table: "HeightWeightModels",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkewnessBMI",
                table: "HeightWeightModels");

            migrationBuilder.DropColumn(
                name: "SkewnessHeight",
                table: "HeightWeightModels");

            migrationBuilder.DropColumn(
                name: "SkewnessWeight",
                table: "HeightWeightModels");
        }
    }
}
