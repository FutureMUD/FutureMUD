using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CellForeignKeyUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveJobs_Characters",
                table: "ActiveJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveJobs_JobListings",
                table: "ActiveJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionHouses_BankAccounts",
                table: "AuctionHouses");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionHouses_Cells",
                table: "AuctionHouses");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionHouses_EconomicZones",
                table: "AuctionHouses");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_BankAccountTypes",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_BankAccounts",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Banks",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Characters",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Clans",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Shops",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTransactions_BankAccounts",
                table: "BankAccountTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_Banks",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_CanCloseProg",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_CharacterProgs",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_ClanProgs",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_ShopProgs",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankBranches_Banks",
                table: "BankBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_BankBranches_Cells",
                table: "BankBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_BankCurrencyReserves_Banks",
                table: "BankCurrencyReserves");

            migrationBuilder.DropForeignKey(
                name: "FK_BankCurrencyReserves_Currencies",
                table: "BankCurrencyReserves");

            migrationBuilder.DropForeignKey(
                name: "FK_BankExchangeRates_Banks",
                table: "BankExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_BankExchangeRates_Currencies_From",
                table: "BankExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_BankExchangeRates_Currencies_To",
                table: "BankExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_BankManagerAuditLogs_Banks",
                table: "BankManagerAuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_BankManagers_Banks",
                table: "BankManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_BankManagers_Characters",
                table: "BankManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_CellOverlayPackages",
                table: "CellOverlays");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_Cells",
                table: "CellOverlays");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_Terrains",
                table: "CellOverlays");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_Exits_CellOverlays",
                table: "CellOverlays_Exits");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_Exits_Exits",
                table: "CellOverlays_Exits");

            migrationBuilder.DropForeignKey(
                name: "FK_Cells_Rooms",
                table: "Cells");

            migrationBuilder.DropForeignKey(
                name: "FK_Cells_Tags_Cells",
                table: "Cells_Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_ConveyancingLocations_Cells",
                table: "ConveyancingLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ConveyancingLocations_EconomicZones",
                table: "ConveyancingLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_JobFindingLocations_Cells",
                table: "JobFindingLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_JobFindingLocations_EconomicZones",
                table: "JobFindingLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_JobListings_EconomicZones",
                table: "JobListings");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyKeys_GameItems",
                table: "PropertyKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyKeys_Property",
                table: "PropertyKeys");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveJobs_Characters",
                table: "ActiveJobs",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveJobs_JobListings",
                table: "ActiveJobs",
                column: "JobListingId",
                principalTable: "JobListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionHouses_BankAccounts",
                table: "AuctionHouses",
                column: "ProfitsBankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionHouses_Cells",
                table: "AuctionHouses",
                column: "AuctionHouseCellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionHouses_EconomicZones",
                table: "AuctionHouses",
                column: "EconomicZoneId",
                principalTable: "EconomicZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_BankAccountTypes",
                table: "BankAccounts",
                column: "BankAccountTypeId",
                principalTable: "BankAccountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_BankAccounts",
                table: "BankAccounts",
                column: "NominatedBenefactorAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Banks",
                table: "BankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Characters",
                table: "BankAccounts",
                column: "AccountOwnerCharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Clans",
                table: "BankAccounts",
                column: "AccountOwnerClanId",
                principalTable: "Clans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Shops",
                table: "BankAccounts",
                column: "AccountOwnerShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTransactions_BankAccounts",
                table: "BankAccountTransactions",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_Banks",
                table: "BankAccountTypes",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_CanCloseProg",
                table: "BankAccountTypes",
                column: "CanCloseAccountProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_CharacterProgs",
                table: "BankAccountTypes",
                column: "CanOpenAccountProgCharacterId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_ClanProgs",
                table: "BankAccountTypes",
                column: "CanOpenAccountProgClanId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_ShopProgs",
                table: "BankAccountTypes",
                column: "CanOpenAccountProgShopId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankBranches_Banks",
                table: "BankBranches",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankBranches_Cells",
                table: "BankBranches",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankCurrencyReserves_Banks",
                table: "BankCurrencyReserves",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankCurrencyReserves_Currencies",
                table: "BankCurrencyReserves",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankExchangeRates_Banks",
                table: "BankExchangeRates",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankExchangeRates_Currencies_From",
                table: "BankExchangeRates",
                column: "FromCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankExchangeRates_Currencies_To",
                table: "BankExchangeRates",
                column: "ToCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankManagerAuditLogs_Banks",
                table: "BankManagerAuditLogs",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankManagers_Banks",
                table: "BankManagers",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankManagers_Characters",
                table: "BankManagers",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_CellOverlayPackages",
                table: "CellOverlays",
                columns: new[] { "CellOverlayPackageId", "CellOverlayPackageRevisionNumber" },
                principalTable: "CellOverlayPackages",
                principalColumns: new[] { "Id", "RevisionNumber" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_Cells",
                table: "CellOverlays",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_Terrains",
                table: "CellOverlays",
                column: "TerrainId",
                principalTable: "Terrains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_Exits_CellOverlays",
                table: "CellOverlays_Exits",
                column: "CellOverlayId",
                principalTable: "CellOverlays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_Exits_Exits",
                table: "CellOverlays_Exits",
                column: "ExitId",
                principalTable: "Exits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cells_Rooms",
                table: "Cells",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cells_Tags_Cells",
                table: "Cells_Tags",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConveyancingLocations_Cells",
                table: "ConveyancingLocations",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConveyancingLocations_EconomicZones",
                table: "ConveyancingLocations",
                column: "EconomicZoneId",
                principalTable: "EconomicZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobFindingLocations_Cells",
                table: "JobFindingLocations",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobFindingLocations_EconomicZones",
                table: "JobFindingLocations",
                column: "EconomicZoneId",
                principalTable: "EconomicZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobListings_EconomicZones",
                table: "JobListings",
                column: "EconomicZoneId",
                principalTable: "EconomicZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyKeys_GameItems",
                table: "PropertyKeys",
                column: "GameItemId",
                principalTable: "GameItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyKeys_Property",
                table: "PropertyKeys",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveJobs_Characters",
                table: "ActiveJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveJobs_JobListings",
                table: "ActiveJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionHouses_BankAccounts",
                table: "AuctionHouses");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionHouses_Cells",
                table: "AuctionHouses");

            migrationBuilder.DropForeignKey(
                name: "FK_AuctionHouses_EconomicZones",
                table: "AuctionHouses");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_BankAccountTypes",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_BankAccounts",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Banks",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Characters",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Clans",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Shops",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTransactions_BankAccounts",
                table: "BankAccountTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_Banks",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_CanCloseProg",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_CharacterProgs",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_ClanProgs",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountTypes_ShopProgs",
                table: "BankAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BankBranches_Banks",
                table: "BankBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_BankBranches_Cells",
                table: "BankBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_BankCurrencyReserves_Banks",
                table: "BankCurrencyReserves");

            migrationBuilder.DropForeignKey(
                name: "FK_BankCurrencyReserves_Currencies",
                table: "BankCurrencyReserves");

            migrationBuilder.DropForeignKey(
                name: "FK_BankExchangeRates_Banks",
                table: "BankExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_BankExchangeRates_Currencies_From",
                table: "BankExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_BankExchangeRates_Currencies_To",
                table: "BankExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_BankManagerAuditLogs_Banks",
                table: "BankManagerAuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_BankManagers_Banks",
                table: "BankManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_BankManagers_Characters",
                table: "BankManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_CellOverlayPackages",
                table: "CellOverlays");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_Cells",
                table: "CellOverlays");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_Terrains",
                table: "CellOverlays");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_Exits_CellOverlays",
                table: "CellOverlays_Exits");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_Exits_Exits",
                table: "CellOverlays_Exits");

            migrationBuilder.DropForeignKey(
                name: "FK_Cells_Rooms",
                table: "Cells");

            migrationBuilder.DropForeignKey(
                name: "FK_Cells_Tags_Cells",
                table: "Cells_Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_ConveyancingLocations_Cells",
                table: "ConveyancingLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ConveyancingLocations_EconomicZones",
                table: "ConveyancingLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_JobFindingLocations_Cells",
                table: "JobFindingLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_JobFindingLocations_EconomicZones",
                table: "JobFindingLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_JobListings_EconomicZones",
                table: "JobListings");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyKeys_GameItems",
                table: "PropertyKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyKeys_Property",
                table: "PropertyKeys");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveJobs_Characters",
                table: "ActiveJobs",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveJobs_JobListings",
                table: "ActiveJobs",
                column: "JobListingId",
                principalTable: "JobListings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionHouses_BankAccounts",
                table: "AuctionHouses",
                column: "ProfitsBankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionHouses_Cells",
                table: "AuctionHouses",
                column: "AuctionHouseCellId",
                principalTable: "Cells",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionHouses_EconomicZones",
                table: "AuctionHouses",
                column: "EconomicZoneId",
                principalTable: "EconomicZones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_BankAccountTypes",
                table: "BankAccounts",
                column: "BankAccountTypeId",
                principalTable: "BankAccountTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_BankAccounts",
                table: "BankAccounts",
                column: "NominatedBenefactorAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Banks",
                table: "BankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Characters",
                table: "BankAccounts",
                column: "AccountOwnerCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Clans",
                table: "BankAccounts",
                column: "AccountOwnerClanId",
                principalTable: "Clans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Shops",
                table: "BankAccounts",
                column: "AccountOwnerShopId",
                principalTable: "Shops",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTransactions_BankAccounts",
                table: "BankAccountTransactions",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_Banks",
                table: "BankAccountTypes",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_CanCloseProg",
                table: "BankAccountTypes",
                column: "CanCloseAccountProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_CharacterProgs",
                table: "BankAccountTypes",
                column: "CanOpenAccountProgCharacterId",
                principalTable: "FutureProgs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_ClanProgs",
                table: "BankAccountTypes",
                column: "CanOpenAccountProgClanId",
                principalTable: "FutureProgs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountTypes_ShopProgs",
                table: "BankAccountTypes",
                column: "CanOpenAccountProgShopId",
                principalTable: "FutureProgs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankBranches_Banks",
                table: "BankBranches",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankBranches_Cells",
                table: "BankBranches",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankCurrencyReserves_Banks",
                table: "BankCurrencyReserves",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankCurrencyReserves_Currencies",
                table: "BankCurrencyReserves",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankExchangeRates_Banks",
                table: "BankExchangeRates",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankExchangeRates_Currencies_From",
                table: "BankExchangeRates",
                column: "FromCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankExchangeRates_Currencies_To",
                table: "BankExchangeRates",
                column: "ToCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankManagerAuditLogs_Banks",
                table: "BankManagerAuditLogs",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankManagers_Banks",
                table: "BankManagers",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankManagers_Characters",
                table: "BankManagers",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_CellOverlayPackages",
                table: "CellOverlays",
                columns: new[] { "CellOverlayPackageId", "CellOverlayPackageRevisionNumber" },
                principalTable: "CellOverlayPackages",
                principalColumns: new[] { "Id", "RevisionNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_Cells",
                table: "CellOverlays",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_Terrains",
                table: "CellOverlays",
                column: "TerrainId",
                principalTable: "Terrains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_Exits_CellOverlays",
                table: "CellOverlays_Exits",
                column: "CellOverlayId",
                principalTable: "CellOverlays",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_Exits_Exits",
                table: "CellOverlays_Exits",
                column: "ExitId",
                principalTable: "Exits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cells_Rooms",
                table: "Cells",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cells_Tags_Cells",
                table: "Cells_Tags",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConveyancingLocations_Cells",
                table: "ConveyancingLocations",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConveyancingLocations_EconomicZones",
                table: "ConveyancingLocations",
                column: "EconomicZoneId",
                principalTable: "EconomicZones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobFindingLocations_Cells",
                table: "JobFindingLocations",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobFindingLocations_EconomicZones",
                table: "JobFindingLocations",
                column: "EconomicZoneId",
                principalTable: "EconomicZones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobListings_EconomicZones",
                table: "JobListings",
                column: "EconomicZoneId",
                principalTable: "EconomicZones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyKeys_GameItems",
                table: "PropertyKeys",
                column: "GameItemId",
                principalTable: "GameItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyKeys_Property",
                table: "PropertyKeys",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id");
        }
    }
}
