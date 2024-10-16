using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class BodyOverrideHealthStrategy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HealthStrategyId",
                table: "Bodies",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bodies_HealthStrategyId",
                table: "Bodies",
                column: "HealthStrategyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bodies_HealthStrategies_HealthStrategyId",
                table: "Bodies",
                column: "HealthStrategyId",
                principalTable: "HealthStrategies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bodies_HealthStrategies_HealthStrategyId",
                table: "Bodies");

            migrationBuilder.DropIndex(
                name: "IX_Bodies_HealthStrategyId",
                table: "Bodies");

            migrationBuilder.DropColumn(
                name: "HealthStrategyId",
                table: "Bodies");
        }
    }
}
