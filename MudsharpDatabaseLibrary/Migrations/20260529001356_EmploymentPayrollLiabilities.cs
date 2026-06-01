using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class EmploymentPayrollLiabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmploymentPayables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RuntimeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CorrelationId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ContractRuntimeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EmployeeName = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Role = table.Column<int>(type: "int(11)", nullable: false),
                    AmountCurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    PayCadence = table.Column<int>(type: "int(11)", nullable: false),
                    PaymentMethodKind = table.Column<int>(type: "int(11)", nullable: false),
                    PaymentBankAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaymentItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaymentItemType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PaymentNotes = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PayPeriodStart = table.Column<DateTime>(type: "datetime", nullable: false),
                    PayPeriodEnd = table.Column<DateTime>(type: "datetime", nullable: false),
                    DueAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccruedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    SettledAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClaimedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    SettlementNote = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentPayables_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentPayables_HostStates_idx",
                table: "EmploymentPayables",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentPayables_Contract_Period",
                table: "EmploymentPayables",
                columns: new[] { "EmploymentHostStateId", "ContractRuntimeId", "PayPeriodStart", "PayPeriodEnd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentPayables_Correlation",
                table: "EmploymentPayables",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentPayables_Employee",
                table: "EmploymentPayables",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentPayables_Host_Runtime",
                table: "EmploymentPayables",
                columns: new[] { "EmploymentHostStateId", "RuntimeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmploymentPayables");
        }
    }
}
