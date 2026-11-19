using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations;

/// <inheritdoc />
public partial class _20261119035119_RecordManualSentenceMinimumOverride : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ManualSentenceMinimumOverride",
            table: "Crimes",
            type: "int(11)",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ManualSentenceMinimumOverride",
            table: "Crimes");
    }
}
