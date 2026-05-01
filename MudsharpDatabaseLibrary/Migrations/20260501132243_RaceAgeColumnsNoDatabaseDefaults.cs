using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RaceAgeColumnsNoDatabaseDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "YouthAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldDefaultValueSql: "'10'");

            migrationBuilder.AlterColumn<int>(
                name: "YoungAdultAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldDefaultValueSql: "'16'");

            migrationBuilder.AlterColumn<int>(
                name: "VenerableAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldDefaultValueSql: "'75'");

            migrationBuilder.AlterColumn<int>(
                name: "ElderAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldDefaultValueSql: "'55'");

            migrationBuilder.AlterColumn<int>(
                name: "ChildAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldDefaultValueSql: "'3'");

            migrationBuilder.AlterColumn<int>(
                name: "AdultAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldDefaultValueSql: "'21'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "YouthAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'10'",
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AlterColumn<int>(
                name: "YoungAdultAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'16'",
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AlterColumn<int>(
                name: "VenerableAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'75'",
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AlterColumn<int>(
                name: "ElderAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'55'",
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AlterColumn<int>(
                name: "ChildAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'3'",
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AlterColumn<int>(
                name: "AdultAge",
                table: "Races",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'21'",
                oldClrType: typeof(int),
                oldType: "int(11)");
        }
    }
}
