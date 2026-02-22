using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ArenaNpcCompletionRestore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "FullyRestoreNpcOnCompletion",
                table: "ArenaCombatantClasses",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullyRestoreNpcOnCompletion",
                table: "ArenaCombatantClasses");
        }
    }
}
