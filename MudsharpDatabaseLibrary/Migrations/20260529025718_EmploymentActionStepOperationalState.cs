using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class EmploymentActionStepOperationalState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CraftJobReference",
                table: "EmploymentActiveTaskStepStates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FailureDiagnostic",
                table: "EmploymentActiveTaskStepStates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LoadedAssets",
                table: "EmploymentActiveTaskStepStates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OperationalPayload",
                table: "EmploymentActiveTaskStepStates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReservationReference",
                table: "EmploymentActiveTaskStepStates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RouteResult",
                table: "EmploymentActiveTaskStepStates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SelectedResources",
                table: "EmploymentActiveTaskStepStates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TransactionReference",
                table: "EmploymentActiveTaskStepStates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CraftJobReference",
                table: "EmploymentActiveTaskStepStates");

            migrationBuilder.DropColumn(
                name: "FailureDiagnostic",
                table: "EmploymentActiveTaskStepStates");

            migrationBuilder.DropColumn(
                name: "LoadedAssets",
                table: "EmploymentActiveTaskStepStates");

            migrationBuilder.DropColumn(
                name: "OperationalPayload",
                table: "EmploymentActiveTaskStepStates");

            migrationBuilder.DropColumn(
                name: "ReservationReference",
                table: "EmploymentActiveTaskStepStates");

            migrationBuilder.DropColumn(
                name: "RouteResult",
                table: "EmploymentActiveTaskStepStates");

            migrationBuilder.DropColumn(
                name: "SelectedResources",
                table: "EmploymentActiveTaskStepStates");

            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "EmploymentActiveTaskStepStates");
        }
    }
}
