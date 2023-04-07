using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class JusticeOverhaulOct21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumFine",
                table: "Laws");

            migrationBuilder.DropColumn(
                name: "MinimumFine",
                table: "Laws");

            migrationBuilder.DropColumn(
                name: "StandardFine",
                table: "Laws");

            migrationBuilder.AddColumn<double>(
                name: "AutomaticConvictionTime",
                table: "LegalAuthorities",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "AutomaticallyConvict",
                table: "LegalAuthorities",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "BailCalculationProgId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BankAccountId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CourtLocationId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GuardianDiscordChannel",
                table: "LegalAuthorities",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "JailLocationId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OnHoldProgId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PunishmentStrategy",
                table: "Laws",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<decimal>(
                name: "CalculatedBail",
                table: "Crimes",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 0.0m);

            migrationBuilder.AddColumn<double>(
                name: "CustodialSentenceLength",
                table: "Crimes",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "FineRecorded",
                table: "Crimes",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 0.0m);

            migrationBuilder.AddColumn<ulong>(
                name: "HasBeenEnforced",
                table: "Crimes",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "LegalAuthorityFines",
                columns: table => new
                {
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FinesOwned = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    PaymentRequiredBy = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.LegalAuthorityId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_LegalAuthorityFines_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalAuthorityFines_LegalAuthorities",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalAuthorityJailCells",
                columns: table => new
                {
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.LegalAuthorityId, x.CellId });
                    table.ForeignKey(
                        name: "FK_LegalAuthoritiesCells_Cells_Jail",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalAuthoritiesCells_LegalAuthorities_Jail",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegalAuthorities_BailCalculationProgId",
                table: "LegalAuthorities",
                column: "BailCalculationProgId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalAuthorities_BankAccountId",
                table: "LegalAuthorities",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalAuthorities_CourtLocationId",
                table: "LegalAuthorities",
                column: "CourtLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalAuthorities_JailLocationId",
                table: "LegalAuthorities",
                column: "JailLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalAuthorities_OnHoldProgId",
                table: "LegalAuthorities",
                column: "OnHoldProgId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalAuthorityFines_CharacterId",
                table: "LegalAuthorityFines",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthoritiesCells_Cells_Jail_idx",
                table: "LegalAuthorityJailCells",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthoritiesCells_LegalAuthorities_Jail_idx",
                table: "LegalAuthorityJailCells",
                column: "LegalAuthorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_BankAccounts_BankAccountId",
                table: "LegalAuthorities",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_CourtroomCell",
                table: "LegalAuthorities",
                column: "CourtLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_FutureprogsBailCalc",
                table: "LegalAuthorities",
                column: "BailCalculationProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_FutureprogsHold",
                table: "LegalAuthorities",
                column: "OnHoldProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_PrisonJailCells",
                table: "LegalAuthorities",
                column: "JailLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_BankAccounts_BankAccountId",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_CourtroomCell",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_FutureprogsBailCalc",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_FutureprogsHold",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_PrisonJailCells",
                table: "LegalAuthorities");

            migrationBuilder.DropTable(
                name: "LegalAuthorityFines");

            migrationBuilder.DropTable(
                name: "LegalAuthorityJailCells");

            migrationBuilder.DropIndex(
                name: "IX_LegalAuthorities_BailCalculationProgId",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "IX_LegalAuthorities_BankAccountId",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "IX_LegalAuthorities_CourtLocationId",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "IX_LegalAuthorities_JailLocationId",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "IX_LegalAuthorities_OnHoldProgId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "AutomaticConvictionTime",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "AutomaticallyConvict",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "BailCalculationProgId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "BankAccountId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "CourtLocationId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "GuardianDiscordChannel",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "JailLocationId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "OnHoldProgId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "PunishmentStrategy",
                table: "Laws");

            migrationBuilder.DropColumn(
                name: "CalculatedBail",
                table: "Crimes");

            migrationBuilder.DropColumn(
                name: "CustodialSentenceLength",
                table: "Crimes");

            migrationBuilder.DropColumn(
                name: "FineRecorded",
                table: "Crimes");

            migrationBuilder.DropColumn(
                name: "HasBeenEnforced",
                table: "Crimes");

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumFine",
                table: "Laws",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumFine",
                table: "Laws",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StandardFine",
                table: "Laws",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
