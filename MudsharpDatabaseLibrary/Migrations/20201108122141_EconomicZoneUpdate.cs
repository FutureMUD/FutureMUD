using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class EconomicZoneUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalRevenueHeld",
                table: "EconomicZones",
                type: "decimal(10,0)",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.CreateTable(
                name: "EconomicZoneTaxes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: true),
                    MerchantDescription = table.Column<string>(type: "varchar(200)", nullable: true),
                    MerchandiseFilterProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TaxType = table.Column<string>(type: "varchar(50)", nullable: true),
                    Definition = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EconomicZoneTaxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EconomicZoneTaxes_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EconomicZoneTaxes_FutureProgs",
                        column: x => x.MerchandiseFilterProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZoneTaxes_EconomicZones_idx",
                table: "EconomicZoneTaxes",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZoneTaxes_FutureProgs_idx",
                table: "EconomicZoneTaxes",
                column: "MerchandiseFilterProgId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EconomicZoneTaxes");

            migrationBuilder.DropColumn(
                name: "TotalRevenueHeld",
                table: "EconomicZones");
        }
    }
}
