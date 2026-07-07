using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ActiveProjectPaymentSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PaymentCurrencyId",
                table: "ActiveProjects",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentPerUnit",
                table: "ActiveProjectMaterials",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentPerHour",
                table: "ActiveProjectLabours",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ProjectPayables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActiveProjectId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ProjectDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProjectRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    ProjectName = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ProjectOwnerCharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    PayableType = table.Column<int>(type: "int(11)", nullable: false),
                    ProjectLabourRequirementId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RequirementName = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Reason = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EarnedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    ClaimedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClaimedBankAccountId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectPayables_BankAccounts",
                        column: x => x.ClaimedBankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectPayables_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveProjects_PaymentCurrencyId",
                table: "ActiveProjects",
                column: "PaymentCurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectPayables_BankAccounts_idx",
                table: "ProjectPayables",
                column: "ClaimedBankAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectPayables_Currencies_idx",
                table: "ProjectPayables",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayables_ActiveProjectId",
                table: "ProjectPayables",
                column: "ActiveProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayables_Character_Claimed",
                table: "ProjectPayables",
                columns: new[] { "CharacterId", "ClaimedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveProjects_PaymentCurrencies",
                table: "ActiveProjects",
                column: "PaymentCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveProjects_PaymentCurrencies",
                table: "ActiveProjects");

            migrationBuilder.DropTable(
                name: "ProjectPayables");

            migrationBuilder.DropIndex(
                name: "IX_ActiveProjects_PaymentCurrencyId",
                table: "ActiveProjects");

            migrationBuilder.DropColumn(
                name: "PaymentCurrencyId",
                table: "ActiveProjects");

            migrationBuilder.DropColumn(
                name: "PaymentPerUnit",
                table: "ActiveProjectMaterials");

            migrationBuilder.DropColumn(
                name: "PaymentPerHour",
                table: "ActiveProjectLabours");
        }
    }
}
