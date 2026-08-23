using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantServiceSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    ShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AutomatedService = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 0ul),
                    SimulateCrafting = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 0ul),
                    HandlingSeconds = table.Column<int>(type: "int(11)", nullable: false, defaultValue: 15),
                    MaximumBatchWaitSeconds = table.Column<int>(type: "int(11)", nullable: false, defaultValue: 90)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ShopId);
                    table.ForeignKey(
                        name: "FK_Restaurants_Shops",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantCells",
                columns: table => new
                {
                    RestaurantShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Role = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RestaurantShopId, x.CellId, x.Role });
                    table.ForeignKey(
                        name: "FK_RestaurantCells_Restaurants",
                        column: x => x.RestaurantShopId,
                        principalTable: "Restaurants",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantMenuItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RestaurantShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MerchandiseId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    FulfilmentMode = table.Column<int>(type: "int(11)", nullable: false),
                    IsActive = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    DineInAvailable = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    TakeawayAvailable = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    PreparationSeconds = table.Column<int>(type: "int(11)", nullable: false),
                    CraftId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CraftRevisionNumber = table.Column<int>(type: "int(11)", nullable: true),
                    ServingContainerPrototypeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ServingContainerPrototypeRevisionNumber = table.Column<int>(type: "int(11)", nullable: true),
                    TakeawayContainerPrototypeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TakeawayContainerPrototypeRevisionNumber = table.Column<int>(type: "int(11)", nullable: true),
                    TakeawayBagPrototypeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TakeawayBagPrototypeRevisionNumber = table.Column<int>(type: "int(11)", nullable: true),
                    SortOrder = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantMenuItems_Merchandises",
                        column: x => x.MerchandiseId,
                        principalTable: "Merchandises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestaurantMenuItems_Restaurants",
                        column: x => x.RestaurantShopId,
                        principalTable: "Restaurants",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantTables",
                columns: table => new
                {
                    RestaurantShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RestaurantShopId, x.GameItemId });
                    table.ForeignKey(
                        name: "FK_RestaurantTables_Restaurants",
                        column: x => x.RestaurantShopId,
                        principalTable: "Restaurants",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantTableSessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RestaurantShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TableGameItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AbandonmentPendingAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AbandonmentReported = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 0ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantTableSessions_Restaurants",
                        column: x => x.RestaurantShopId,
                        principalTable: "Restaurants",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RestaurantShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RestaurantTableSessionId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RestaurantMenuItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OrderType = table.Column<int>(type: "int(11)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    OrdererCharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OrdererCharacterName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RecipientCharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RecipientCharacterName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Quantity = table.Column<int>(type: "int(11)", nullable: false),
                    PretaxPrice = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    RevenueRecognised = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 0ul),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpectedReadyAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReadyAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ServedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PreparedByEmployeeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ServedByEmployeeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OperationalNotes = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantOrders_MenuItems",
                        column: x => x.RestaurantMenuItemId,
                        principalTable: "RestaurantMenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestaurantOrders_Restaurants",
                        column: x => x.RestaurantShopId,
                        principalTable: "Restaurants",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestaurantOrders_Sessions",
                        column: x => x.RestaurantTableSessionId,
                        principalTable: "RestaurantTableSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantTableParticipants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RestaurantTableSessionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Accepted = table.Column<ulong>(type: "bit(1)", nullable: false),
                    JoinedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LeftAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantTableParticipants_Sessions",
                        column: x => x.RestaurantTableSessionId,
                        principalTable: "RestaurantTableSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantOrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RestaurantOrderId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Role = table.Column<int>(type: "int(11)", nullable: false),
                    Delivered = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 0ul),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeliveredAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantOrderItems_Orders",
                        column: x => x.RestaurantOrderId,
                        principalTable: "RestaurantOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RestaurantOrderId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PayerCharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PayerCharacterName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    IsRefund = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 0ul),
                    PaymentMethod = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Reference = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PaidAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantPayments_Orders",
                        column: x => x.RestaurantOrderId,
                        principalTable: "RestaurantOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCells_Cell",
                table: "RestaurantCells",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCells_Restaurant_Role",
                table: "RestaurantCells",
                columns: new[] { "RestaurantShopId", "Role" });

            migrationBuilder.CreateIndex(
                name: "FK_RestaurantMenuItems_Merchandises_idx",
                table: "RestaurantMenuItems",
                column: "MerchandiseId");

            migrationBuilder.CreateIndex(
                name: "FK_RestaurantMenuItems_Restaurants_idx",
                table: "RestaurantMenuItems",
                column: "RestaurantShopId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantMenuItems_Restaurant_Sort",
                table: "RestaurantMenuItems",
                columns: new[] { "RestaurantShopId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "FK_RestaurantOrderItems_Orders_idx",
                table: "RestaurantOrderItems",
                column: "RestaurantOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantOrderItems_GameItem",
                table: "RestaurantOrderItems",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_RestaurantOrders_MenuItems_idx",
                table: "RestaurantOrders",
                column: "RestaurantMenuItemId");

            migrationBuilder.CreateIndex(
                name: "FK_RestaurantOrders_Restaurants_idx",
                table: "RestaurantOrders",
                column: "RestaurantShopId");

            migrationBuilder.CreateIndex(
                name: "FK_RestaurantOrders_Sessions_idx",
                table: "RestaurantOrders",
                column: "RestaurantTableSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantOrders_Restaurant_Status",
                table: "RestaurantOrders",
                columns: new[] { "RestaurantShopId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantOrders_Session_Debtor",
                table: "RestaurantOrders",
                columns: new[] { "RestaurantTableSessionId", "OrdererCharacterId" });

            migrationBuilder.CreateIndex(
                name: "FK_RestaurantPayments_Orders_idx",
                table: "RestaurantPayments",
                column: "RestaurantOrderId");

            migrationBuilder.CreateIndex(
                name: "FK_RestaurantTableParticipants_Sessions_idx",
                table: "RestaurantTableParticipants",
                column: "RestaurantTableSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantTableParticipants_Session_Character",
                table: "RestaurantTableParticipants",
                columns: new[] { "RestaurantTableSessionId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantTables_GameItem",
                table: "RestaurantTables",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_RestaurantTableSessions_Restaurants_idx",
                table: "RestaurantTableSessions",
                column: "RestaurantShopId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantTableSessions_Table_Status",
                table: "RestaurantTableSessions",
                columns: new[] { "RestaurantShopId", "TableGameItemId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantCells");

            migrationBuilder.DropTable(
                name: "RestaurantOrderItems");

            migrationBuilder.DropTable(
                name: "RestaurantPayments");

            migrationBuilder.DropTable(
                name: "RestaurantTableParticipants");

            migrationBuilder.DropTable(
                name: "RestaurantTables");

            migrationBuilder.DropTable(
                name: "RestaurantOrders");

            migrationBuilder.DropTable(
                name: "RestaurantMenuItems");

            migrationBuilder.DropTable(
                name: "RestaurantTableSessions");

            migrationBuilder.DropTable(
                name: "Restaurants");
        }
    }
}
