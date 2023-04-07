using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class AuctionHouses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuctionHouses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AuctionHouseCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProfitsBankAccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AuctionListingFeeFlat = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    AuctionListingFeeRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DefaultListingTime = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionHouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionHouses_BankAccounts",
                        column: x => x.ProfitsBankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuctionHouses_Cells",
                        column: x => x.AuctionHouseCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuctionHouses_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuctionHouses_AuctionHouseCellId",
                table: "AuctionHouses",
                column: "AuctionHouseCellId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionHouses_EconomicZoneId",
                table: "AuctionHouses",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionHouses_ProfitsBankAccountId",
                table: "AuctionHouses",
                column: "ProfitsBankAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionHouses");
        }
    }
}
