using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class HeightWeightModelDirectSetWeights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MeanWeight",
                table: "HeightWeightModels",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "StddevWeight",
                table: "HeightWeightModels",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeanWeight",
                table: "HeightWeightModels");

            migrationBuilder.DropColumn(
                name: "StddevWeight",
                table: "HeightWeightModels");
        }
    }
}
