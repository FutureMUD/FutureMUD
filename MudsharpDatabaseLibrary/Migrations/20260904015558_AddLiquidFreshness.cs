using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddLiquidFreshness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SpoilAfterSeconds",
                table: "Liquids",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SpoiledLiquidId",
                table: "Liquids",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "StaleAfterSeconds",
                table: "Liquids",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StaleLiquidId",
                table: "Liquids",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_Liquids_SpoiledLiquid_idx",
                table: "Liquids",
                column: "SpoiledLiquidId");

            migrationBuilder.CreateIndex(
                name: "FK_Liquids_StaleLiquid_idx",
                table: "Liquids",
                column: "StaleLiquidId");

            migrationBuilder.AddForeignKey(
                name: "FK_Liquids_SpoiledLiquid",
                table: "Liquids",
                column: "SpoiledLiquidId",
                principalTable: "Liquids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Liquids_StaleLiquid",
                table: "Liquids",
                column: "StaleLiquidId",
                principalTable: "Liquids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Liquids_SpoiledLiquid",
                table: "Liquids");

            migrationBuilder.DropForeignKey(
                name: "FK_Liquids_StaleLiquid",
                table: "Liquids");

            migrationBuilder.DropIndex(
                name: "FK_Liquids_SpoiledLiquid_idx",
                table: "Liquids");

            migrationBuilder.DropIndex(
                name: "FK_Liquids_StaleLiquid_idx",
                table: "Liquids");

            migrationBuilder.DropColumn(
                name: "SpoilAfterSeconds",
                table: "Liquids");

            migrationBuilder.DropColumn(
                name: "SpoiledLiquidId",
                table: "Liquids");

            migrationBuilder.DropColumn(
                name: "StaleAfterSeconds",
                table: "Liquids");

            migrationBuilder.DropColumn(
                name: "StaleLiquidId",
                table: "Liquids");
        }
    }
}
