using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class PersistEmploymentTaskApprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalPrincipalJson",
                table: "EmploymentScheduledTaskRules",
                type: "longtext",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalConfigurationJson",
                table: "EmploymentManagerGoals",
                type: "longtext",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "ProvenanceJson",
                table: "EmploymentActiveTasks",
                type: "longtext",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalPrincipalJson",
                table: "EmploymentScheduledTaskRules");

            migrationBuilder.DropColumn(
                name: "ApprovalConfigurationJson",
                table: "EmploymentManagerGoals");

            migrationBuilder.DropColumn(
                name: "ProvenanceJson",
                table: "EmploymentActiveTasks");
        }
    }
}
