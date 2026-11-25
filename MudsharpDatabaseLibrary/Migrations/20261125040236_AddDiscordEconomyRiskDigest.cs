using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations;

/// <inheritdoc />
public partial class _20261125040236_AddDiscordEconomyRiskDigest : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "EconomyRiskDigestState",
            table: "DiscordAccountLinks",
            type: "mediumtext",
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "EconomyRiskDigestState",
            table: "DiscordAccountLinks");
    }
}
