using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class HotelNormalizedPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelDefinition",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "HotelDefinition",
                table: "Hotels");

            migrationBuilder.CreateTable(
                name: "HotelBannedPatrons",
                columns: table => new
                {
                    HotelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PatronId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.HotelId, x.PatronId });
                    table.ForeignKey(
                        name: "FK_HotelBannedPatrons_Hotels",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HotelPatronBalances",
                columns: table => new
                {
                    HotelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PatronId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.HotelId, x.PatronId });
                    table.ForeignKey(
                        name: "FK_HotelPatronBalances_Hotels",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HotelRooms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HotelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Listed = table.Column<ulong>(type: "bit(1)", nullable: false),
                    PricePerDay = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    SecurityDeposit = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    MinimumDurationTicks = table.Column<long>(type: "bigint(20)", nullable: false),
                    MaximumDurationTicks = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelRooms_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HotelRooms_Hotels",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HotelLostProperties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HotelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    HotelRoomId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BundleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    StoredUntil = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    AuctionHouseId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ReservePrice = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelLostProperties_HotelRooms",
                        column: x => x.HotelRoomId,
                        principalTable: "HotelRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HotelLostProperties_Hotels",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HotelRoomFurnishings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HotelRoomId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ReplacementValue = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    OriginalCondition = table.Column<double>(type: "double", nullable: false),
                    OriginalDamageCondition = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelRoomFurnishings_HotelRooms",
                        column: x => x.HotelRoomId,
                        principalTable: "HotelRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HotelRoomKeys",
                columns: table => new
                {
                    HotelRoomId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PropertyKeyId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.HotelRoomId, x.PropertyKeyId });
                    table.ForeignKey(
                        name: "FK_HotelRoomKeys_HotelRooms",
                        column: x => x.HotelRoomId,
                        principalTable: "HotelRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HotelRoomKeys_PropertyKeys",
                        column: x => x.PropertyKeyId,
                        principalTable: "PropertyKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HotelRoomRentals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HotelRoomId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GuestId = table.Column<long>(type: "bigint(20)", nullable: false),
                    StartTime = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EndTime = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RentalCharge = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    SecurityDeposit = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    TaxCharged = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelRoomRentals_HotelRooms",
                        column: x => x.HotelRoomId,
                        principalTable: "HotelRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HotelBannedPatrons_Patron",
                table: "HotelBannedPatrons",
                column: "PatronId");

            migrationBuilder.CreateIndex(
                name: "FK_HotelLostProperties_HotelRooms_idx",
                table: "HotelLostProperties",
                column: "HotelRoomId");

            migrationBuilder.CreateIndex(
                name: "FK_HotelLostProperties_Hotels_idx",
                table: "HotelLostProperties",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelLostProperties_Bundle",
                table: "HotelLostProperties",
                column: "BundleId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelLostProperties_Owner",
                table: "HotelLostProperties",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelPatronBalances_Patron",
                table: "HotelPatronBalances",
                column: "PatronId");

            migrationBuilder.CreateIndex(
                name: "FK_HotelRoomFurnishings_HotelRooms_idx",
                table: "HotelRoomFurnishings",
                column: "HotelRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelRoomFurnishings_GameItem",
                table: "HotelRoomFurnishings",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_HotelRoomKeys_PropertyKeys_idx",
                table: "HotelRoomKeys",
                column: "PropertyKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelRoomRentals_Guest",
                table: "HotelRoomRentals",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelRoomRentals_Room",
                table: "HotelRoomRentals",
                column: "HotelRoomId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_HotelRooms_Cells_idx",
                table: "HotelRooms",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_HotelRooms_Hotels_idx",
                table: "HotelRooms",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelRooms_Hotel_Cell",
                table: "HotelRooms",
                columns: new[] { "HotelId", "CellId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelBannedPatrons");

            migrationBuilder.DropTable(
                name: "HotelLostProperties");

            migrationBuilder.DropTable(
                name: "HotelPatronBalances");

            migrationBuilder.DropTable(
                name: "HotelRoomFurnishings");

            migrationBuilder.DropTable(
                name: "HotelRoomKeys");

            migrationBuilder.DropTable(
                name: "HotelRoomRentals");

            migrationBuilder.DropTable(
                name: "HotelRooms");

            migrationBuilder.AddColumn<string>(
                name: "HotelDefinition",
                table: "Properties",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "HotelDefinition",
                table: "Hotels",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }
    }
}
