using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ClanHallCellsForEmploymentHosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clans_HallCells",
                columns: table => new
                {
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ClanId, x.CellId });
                    table.ForeignKey(
                        name: "FK_Clans_HallCells_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clans_HallCells_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_Clans_HallCells_Cells_idx",
                table: "Clans_HallCells",
                column: "CellId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clans_HallCells");
        }
    }
}
