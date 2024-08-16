using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ArmourPenaltyToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "StackedDifficultyDegrees",
                table: "ArmourTypes",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AlterColumn<double>(
                name: "BaseDifficultyDegrees",
                table: "ArmourTypes",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StackedDifficultyDegrees",
                table: "ArmourTypes",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<int>(
                name: "BaseDifficultyDegrees",
                table: "ArmourTypes",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");
        }
    }
}
