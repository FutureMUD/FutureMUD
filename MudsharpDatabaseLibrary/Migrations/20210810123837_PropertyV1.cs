using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class PropertyV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConveyancingLocations",
                columns: table => new
                {
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EconomicZoneId, x.CellId });
                    table.ForeignKey(
                        name: "FK_ConveyancingLocations_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConveyancingLocations_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyLeaseOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropertyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PricePerInterval = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    BondRequired = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Interval = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CanLeaseProgCharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CanLeaseProgClanId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MinimumLeaseDurationDays = table.Column<double>(type: "double", nullable: false),
                    MaximumLeaseDurationDays = table.Column<double>(type: "double", nullable: false),
                    AllowAutoRenew = table.Column<ulong>(type: "bit(1)", nullable: false),
                    AutomaticallyRelistAfterLeaseTerm = table.Column<ulong>(type: "bit(1)", nullable: false),
                    AllowLeaseNovation = table.Column<ulong>(type: "bit(1)", nullable: false),
                    ListedForLease = table.Column<ulong>(type: "bit(1)", nullable: false),
                    FeeIncreasePercentageAfterLeaseTerm = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    PropertyOwnerConsentInfo = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyLeaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyLeaseOrders_FutureProgs_Character",
                        column: x => x.CanLeaseProgCharacterId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyLeaseOrders_FutureProgs_Clan",
                        column: x => x.CanLeaseProgClanId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyLeases",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropertyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LeaseOrderId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LeaseholderReference = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PricePerInterval = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    BondPayment = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    PaymentBalance = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    LeaseStart = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LeaseEnd = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LastLeasePayment = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AutoRenew = table.Column<ulong>(type: "bit(1)", nullable: false),
                    BondReturned = table.Column<ulong>(type: "bit(1)", nullable: false),
                    Interval = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    TenantInfo = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyLeases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyLeases_PropertyLeaseOrders",
                        column: x => x.LeaseOrderId,
                        principalTable: "PropertyLeaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyLocations",
                columns: table => new
                {
                    PropertyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.PropertyId, x.CellId });
                    table.ForeignKey(
                        name: "FK_PropertyLocations_Cell",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyOwners",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropertyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FrameworkItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FrameworkItemType = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ShareOfOwnership = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    RevenueAccountId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyOwners_BankAccounts",
                        column: x => x.RevenueAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertySalesOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropertyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ReservePrice = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    OrderStatus = table.Column<int>(type: "int(11)", nullable: false),
                    StartOfListing = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DurationOfListingDays = table.Column<double>(type: "double", nullable: false),
                    PropertyOwnerConsentInfo = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertySalesOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DetailedDescription = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LastChangeOfOwnership = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ApplyCriminalCodeInProperty = table.Column<ulong>(type: "bit(1)", nullable: false),
                    LeaseId = table.Column<long>(type: "bigint(20)", nullable: true),
                    LeaseOrderId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SaleOrderId = table.Column<long>(type: "bigint(20)", nullable: true),
                    LastSaleValue = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Properties_Lease",
                        column: x => x.LeaseId,
                        principalTable: "PropertyLeases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Properties_LeaseOrder",
                        column: x => x.LeaseOrderId,
                        principalTable: "PropertyLeaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Properties_SaleOrder",
                        column: x => x.SaleOrderId,
                        principalTable: "PropertySalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConveyancingLocations_CellId",
                table: "ConveyancingLocations",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_EconomicZoneId",
                table: "Properties",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_LeaseId",
                table: "Properties",
                column: "LeaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_LeaseOrderId",
                table: "Properties",
                column: "LeaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_SaleOrderId",
                table: "Properties",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyLeaseOrders_CanLeaseProgCharacterId",
                table: "PropertyLeaseOrders",
                column: "CanLeaseProgCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyLeaseOrders_CanLeaseProgClanId",
                table: "PropertyLeaseOrders",
                column: "CanLeaseProgClanId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyLeaseOrders_PropertyId",
                table: "PropertyLeaseOrders",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyLeases_LeaseOrderId",
                table: "PropertyLeases",
                column: "LeaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyLeases_PropertyId",
                table: "PropertyLeases",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyLocations_CellId",
                table: "PropertyLocations",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyOwners_PropertyId",
                table: "PropertyOwners",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyOwners_RevenueAccountId",
                table: "PropertyOwners",
                column: "RevenueAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertySalesOrders_PropertyId",
                table: "PropertySalesOrders",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyLeaseOrders_Property",
                table: "PropertyLeaseOrders",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyLeases_Property",
                table: "PropertyLeases",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyLocations_Property",
                table: "PropertyLocations",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyOwners_Properties",
                table: "PropertyOwners",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertySaleOrders_Property",
                table: "PropertySalesOrders",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Lease",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_LeaseOrder",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_SaleOrder",
                table: "Properties");

            migrationBuilder.DropTable(
                name: "ConveyancingLocations");

            migrationBuilder.DropTable(
                name: "PropertyLocations");

            migrationBuilder.DropTable(
                name: "PropertyOwners");

            migrationBuilder.DropTable(
                name: "PropertyLeases");

            migrationBuilder.DropTable(
                name: "PropertyLeaseOrders");

            migrationBuilder.DropTable(
                name: "PropertySalesOrders");

            migrationBuilder.DropTable(
                name: "Properties");
        }
    }
}
