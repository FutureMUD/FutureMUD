using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class PersistEmploymentTaskAuthorisation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssignedAtTicks",
                table: "EmploymentActiveTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorisedByPrincipal",
                table: "EmploymentActiveTasks",
                type: "mediumtext",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "CompletedAtTicks",
                table: "EmploymentActiveTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedAtTicks",
                table: "EmploymentActiveTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByPrincipal",
                table: "EmploymentActiveTasks",
                type: "mediumtext",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "DueAtTicks",
                table: "EmploymentActiveTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FinancialRecoveryRequired",
                table: "EmploymentActiveTasks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "EmploymentActiveTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvenanceVersion",
                table: "EmploymentActiveTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SourceGoalId",
                table: "EmploymentActiveTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceKind",
                table: "EmploymentActiveTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceRuleId",
                table: "EmploymentActiveTasks",
                type: "varchar(36)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "StartedAtTicks",
                table: "EmploymentActiveTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmploymentTaskGrants",
                columns: table => new
                {
                    EmploymentActiveTaskId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    PublicId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmploymentHostStateId = table.Column<long>(type: "bigint", nullable: false),
                    EmploymentActionPlanId = table.Column<long>(type: "bigint", nullable: false),
                    TaskPublicId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CorrelationId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IssuerPrincipal = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Authority = table.Column<long>(type: "bigint", nullable: false),
                    AllowsUnboundedFinancialSteps = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ExpectedLimitCount = table.Column<int>(type: "int", nullable: false),
                    ExpectedPaymentScopeCount = table.Column<int>(type: "int", nullable: false),
                    ExpectedCounterpartyScopeCount = table.Column<int>(type: "int", nullable: false),
                    IssuedAtTicks = table.Column<long>(type: "bigint", nullable: false),
                    ExpiresAtTicks = table.Column<long>(type: "bigint", nullable: true),
                    RevokedAtTicks = table.Column<long>(type: "bigint", nullable: true),
                    RevokedByPrincipal = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RevocationReason = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTaskGrants", x => x.EmploymentActiveTaskId);
                    table.ForeignKey(
                        name: "FK_EmploymentTaskGrants_EmploymentActiveTasks_EmploymentActiveT~",
                        column: x => x.EmploymentActiveTaskId,
                        principalTable: "EmploymentActiveTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentTaskGrantLimits",
                columns: table => new
                {
                    EmploymentActiveTaskId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTaskGrantLimits", x => new { x.EmploymentActiveTaskId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_EmploymentTaskGrantLimits_EmploymentTaskGrants_EmploymentAct~",
                        column: x => x.EmploymentActiveTaskId,
                        principalTable: "EmploymentTaskGrants",
                        principalColumn: "EmploymentActiveTaskId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentTaskGrantScopes",
                columns: table => new
                {
                    EmploymentActiveTaskId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    Scope = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTaskGrantScopes", x => new { x.EmploymentActiveTaskId, x.Kind, x.Scope });
                    table.ForeignKey(
                        name: "FK_EmploymentTaskGrantScopes_EmploymentTaskGrants_EmploymentAct~",
                        column: x => x.EmploymentActiveTaskId,
                        principalTable: "EmploymentTaskGrants",
                        principalColumn: "EmploymentActiveTaskId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentTaskGrants_PublicId",
                table: "EmploymentTaskGrants",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmploymentTaskGrantLimits");

            migrationBuilder.DropTable(
                name: "EmploymentTaskGrantScopes");

            migrationBuilder.DropTable(
                name: "EmploymentTaskGrants");

            migrationBuilder.DropColumn(
                name: "AssignedAtTicks",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "AuthorisedByPrincipal",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "CompletedAtTicks",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "CreatedAtTicks",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "CreatedByPrincipal",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "DueAtTicks",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "FinancialRecoveryRequired",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "ProvenanceVersion",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "SourceGoalId",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "SourceKind",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "SourceRuleId",
                table: "EmploymentActiveTasks");

            migrationBuilder.DropColumn(
                name: "StartedAtTicks",
                table: "EmploymentActiveTasks");
        }
    }
}
