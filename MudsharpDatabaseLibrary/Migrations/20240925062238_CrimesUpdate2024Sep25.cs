using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CrimesUpdate2024Sep25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "HasBeenEnforced",
                table: "Crimes",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "BailHasBeenPosted",
                table: "Crimes",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AddColumn<ulong>(
                name: "ExecutionPunishment",
                table: "Crimes",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "FineHasBeenPaid",
                table: "Crimes",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<double>(
                name: "GoodBehaviourBond",
                table: "Crimes",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<ulong>(
                name: "SentenceHasBeenServed",
                table: "Crimes",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionPunishment",
                table: "Crimes");

            migrationBuilder.DropColumn(
                name: "FineHasBeenPaid",
                table: "Crimes");

            migrationBuilder.DropColumn(
                name: "GoodBehaviourBond",
                table: "Crimes");

            migrationBuilder.DropColumn(
                name: "SentenceHasBeenServed",
                table: "Crimes");

            migrationBuilder.AlterColumn<ulong>(
                name: "HasBeenEnforced",
                table: "Crimes",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)",
                oldDefaultValue: 0ul);

            migrationBuilder.AlterColumn<ulong>(
                name: "BailHasBeenPosted",
                table: "Crimes",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)",
                oldDefaultValue: 0ul);
        }
    }
}
