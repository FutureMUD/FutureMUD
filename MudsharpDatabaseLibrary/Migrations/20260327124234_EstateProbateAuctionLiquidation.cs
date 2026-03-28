using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class EstateProbateAuctionLiquidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "GameItems",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerType",
                table: "GameItems",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "EstateAuctionHouseId",
                table: "EconomicZones",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstateClaimPeriodLength",
                table: "EconomicZones",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "EstateDefaultDiscoverTime",
                table: "EconomicZones",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "EstateHeirId",
                table: "Characters",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstateHeirType",
                table: "Characters",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "AccountOwnerFrameworkItemId",
                table: "BankAccounts",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountOwnerFrameworkItemType",
                table: "BankAccounts",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.Sql(@"
UPDATE `BankAccounts`
SET `AccountOwnerFrameworkItemId` = `AccountOwnerCharacterId`,
	`AccountOwnerFrameworkItemType` = 'Character'
WHERE `AccountOwnerFrameworkItemId` IS NULL
  AND `AccountOwnerCharacterId` IS NOT NULL;

UPDATE `BankAccounts`
SET `AccountOwnerFrameworkItemId` = `AccountOwnerClanId`,
	`AccountOwnerFrameworkItemType` = 'Clan'
WHERE `AccountOwnerFrameworkItemId` IS NULL
  AND `AccountOwnerClanId` IS NOT NULL;

UPDATE `BankAccounts`
SET `AccountOwnerFrameworkItemId` = `AccountOwnerShopId`,
	`AccountOwnerFrameworkItemType` = 'Shop'
WHERE `AccountOwnerFrameworkItemId` IS NULL
  AND `AccountOwnerShopId` IS NOT NULL;
");

            migrationBuilder.CreateTable(
                name: "Estates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EstateStatus = table.Column<int>(type: "int(11)", nullable: false),
                    EstateStartTime = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    FinalisationDate = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    InheritorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    InheritorType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estates_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Estates_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EstateAssets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EstateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FrameworkItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FrameworkItemType = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IsPresumedOwnership = table.Column<ulong>(type: "bit(1)", nullable: false),
                    IsTransferred = table.Column<ulong>(type: "bit(1)", nullable: false),
                    IsLiquidated = table.Column<ulong>(type: "bit(1)", nullable: false),
                    LiquidatedValue = table.Column<decimal>(type: "decimal(58,29)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstateAssets_Estates",
                        column: x => x.EstateId,
                        principalTable: "Estates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EstateClaims",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EstateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ClaimantId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ClaimantType = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    TargetId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TargetType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ClaimStatus = table.Column<int>(type: "int(11)", nullable: false),
                    StatusReason = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IsSecured = table.Column<ulong>(type: "bit(1)", nullable: false),
                    ClaimDate = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstateClaims_Estates",
                        column: x => x.EstateId,
                        principalTable: "Estates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZones_EstateAuctionHouses_idx",
                table: "EconomicZones",
                column: "EstateAuctionHouseId");

            migrationBuilder.CreateIndex(
                name: "FK_EstateAssets_Estates_idx",
                table: "EstateAssets",
                column: "EstateId");

            migrationBuilder.CreateIndex(
                name: "FK_EstateClaims_Estates_idx",
                table: "EstateClaims",
                column: "EstateId");

            migrationBuilder.CreateIndex(
                name: "FK_Estates_Characters_idx",
                table: "Estates",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_Estates_EconomicZones_idx",
                table: "Estates",
                column: "EconomicZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_EconomicZones_EstateAuctionHouses",
                table: "EconomicZones",
                column: "EstateAuctionHouseId",
                principalTable: "AuctionHouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EconomicZones_EstateAuctionHouses",
                table: "EconomicZones");

            migrationBuilder.DropTable(
                name: "EstateAssets");

            migrationBuilder.DropTable(
                name: "EstateClaims");

            migrationBuilder.DropTable(
                name: "Estates");

            migrationBuilder.DropIndex(
                name: "FK_EconomicZones_EstateAuctionHouses_idx",
                table: "EconomicZones");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "GameItems");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                table: "GameItems");

            migrationBuilder.DropColumn(
                name: "EstateAuctionHouseId",
                table: "EconomicZones");

            migrationBuilder.DropColumn(
                name: "EstateClaimPeriodLength",
                table: "EconomicZones");

            migrationBuilder.DropColumn(
                name: "EstateDefaultDiscoverTime",
                table: "EconomicZones");

            migrationBuilder.DropColumn(
                name: "EstateHeirId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "EstateHeirType",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AccountOwnerFrameworkItemId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "AccountOwnerFrameworkItemType",
                table: "BankAccounts");
        }
    }
}
