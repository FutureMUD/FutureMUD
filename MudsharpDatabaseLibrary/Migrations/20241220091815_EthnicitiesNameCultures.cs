using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class EthnicitiesNameCultures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EthnicitiesNameCultures",
                columns: table => new
                {
                    EthnicityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NameCultureId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Gender = table.Column<short>(type: "smallint(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EthnicityId, x.NameCultureId, x.Gender });
                    table.ForeignKey(
                        name: "FK_EthnicitiesNameCultures_Ethnicities",
                        column: x => x.EthnicityId,
                        principalTable: "Ethnicities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EthnicitiesNameCultures_NameCultures",
                        column: x => x.NameCultureId,
                        principalTable: "NameCulture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EthnicitiesNameCultures_NameCultureId",
                table: "EthnicitiesNameCultures",
                column: "NameCultureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthnicitiesNameCultures");
        }
    }
}
