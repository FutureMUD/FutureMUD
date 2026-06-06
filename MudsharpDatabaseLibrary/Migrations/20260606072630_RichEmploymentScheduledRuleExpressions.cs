using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RichEmploymentScheduledRuleExpressions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ConditionPredicateId",
                table: "EmploymentTaskConditions",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ScheduledRuleTemplateId",
                table: "EmploymentTaskConditions",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpressionJson",
                table: "EmploymentScheduledTaskRules",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentConditionPredicates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ExpressionJson = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentConditionPredicates_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentScheduledRuleTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IdempotencyKeyPattern = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmploymentActionPlanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ExpressionJson = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CooldownTicks = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentScheduledRuleTemplates_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentScheduledRuleTemplates_Plans",
                        column: x => x.EmploymentActionPlanId,
                        principalTable: "EmploymentActionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentTaskConditions_ConditionPredicates_idx",
                table: "EmploymentTaskConditions",
                column: "ConditionPredicateId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentTaskConditions_ScheduledRuleTemplates_idx",
                table: "EmploymentTaskConditions",
                column: "ScheduledRuleTemplateId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentConditionPredicates_HostStates_idx",
                table: "EmploymentConditionPredicates",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentConditionPredicates_Host_Name",
                table: "EmploymentConditionPredicates",
                columns: new[] { "EmploymentHostStateId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentConditionPredicates_PublicId",
                table: "EmploymentConditionPredicates",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentScheduledRuleTemplates_HostStates_idx",
                table: "EmploymentScheduledRuleTemplates",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentScheduledRuleTemplates_Plans_idx",
                table: "EmploymentScheduledRuleTemplates",
                column: "EmploymentActionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentScheduledRuleTemplates_Host_Name",
                table: "EmploymentScheduledRuleTemplates",
                columns: new[] { "EmploymentHostStateId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentScheduledRuleTemplates_PublicId",
                table: "EmploymentScheduledRuleTemplates",
                column: "PublicId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentTaskConditions_ConditionPredicates",
                table: "EmploymentTaskConditions",
                column: "ConditionPredicateId",
                principalTable: "EmploymentConditionPredicates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentTaskConditions_ScheduledRuleTemplates",
                table: "EmploymentTaskConditions",
                column: "ScheduledRuleTemplateId",
                principalTable: "EmploymentScheduledRuleTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentTaskConditions_ConditionPredicates",
                table: "EmploymentTaskConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentTaskConditions_ScheduledRuleTemplates",
                table: "EmploymentTaskConditions");

            migrationBuilder.DropTable(
                name: "EmploymentConditionPredicates");

            migrationBuilder.DropTable(
                name: "EmploymentScheduledRuleTemplates");

            migrationBuilder.DropIndex(
                name: "FK_EmploymentTaskConditions_ConditionPredicates_idx",
                table: "EmploymentTaskConditions");

            migrationBuilder.DropIndex(
                name: "FK_EmploymentTaskConditions_ScheduledRuleTemplates_idx",
                table: "EmploymentTaskConditions");

            migrationBuilder.DropColumn(
                name: "ConditionPredicateId",
                table: "EmploymentTaskConditions");

            migrationBuilder.DropColumn(
                name: "ScheduledRuleTemplateId",
                table: "EmploymentTaskConditions");

            migrationBuilder.DropColumn(
                name: "ExpressionJson",
                table: "EmploymentScheduledTaskRules");
        }
    }
}
