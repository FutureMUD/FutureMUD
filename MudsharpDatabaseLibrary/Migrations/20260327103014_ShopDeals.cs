using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ShopDeals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopDeals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DealType = table.Column<int>(type: "int(11)", nullable: false),
                    TargetType = table.Column<int>(type: "int(11)", nullable: false),
                    MerchandiseId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TagId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PriceAdjustmentPercentage = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    MinimumQuantity = table.Column<int>(type: "int(11)", nullable: true),
                    Applicability = table.Column<int>(type: "int(11)", nullable: false),
                    EligibilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ExpiryDateTime = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IsCumulative = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopDeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopDeals_FutureProgs",
                        column: x => x.EligibilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ShopDeals_Merchandises",
                        column: x => x.MerchandiseId,
                        principalTable: "Merchandises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ShopDeals_Shops",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopDeals_Tags",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_ShopDeals_FutureProgs_idx",
                table: "ShopDeals",
                column: "EligibilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ShopDeals_Merchandises_idx",
                table: "ShopDeals",
                column: "MerchandiseId");

            migrationBuilder.CreateIndex(
                name: "FK_ShopDeals_Shops_idx",
                table: "ShopDeals",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "FK_ShopDeals_Tags_idx",
                table: "ShopDeals",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopDeals");
        }
    }
}
