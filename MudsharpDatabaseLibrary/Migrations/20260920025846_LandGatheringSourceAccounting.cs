using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class LandGatheringSourceAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EcologicalApplied",
                table: "MagicGatheringOperations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "EcologicalChildId",
                table: "MagicGatheringOperations",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "LandDetailJson",
                table: "MagicGatheringOperations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MagicGatheringParticipants",
                columns: table => new
                {
                    OperationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceKey = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.OperationId, x.SourceKey });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MagicGatheringParticipants_Cell_Source",
                table: "MagicGatheringParticipants",
                columns: new[] { "CellId", "SourceKey" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MagicGatheringParticipants");

            migrationBuilder.DropColumn(
                name: "EcologicalApplied",
                table: "MagicGatheringOperations");

            migrationBuilder.DropColumn(
                name: "EcologicalChildId",
                table: "MagicGatheringOperations");

            migrationBuilder.DropColumn(
                name: "LandDetailJson",
                table: "MagicGatheringOperations");
        }
    }
}
