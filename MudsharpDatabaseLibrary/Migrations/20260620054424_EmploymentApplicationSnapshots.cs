using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class EmploymentApplicationSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RevisionNumber",
                table: "EmploymentJobOpenings",
                type: "int(11)",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<long>(
                name: "OriginApplicationId",
                table: "EmploymentContracts",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OriginOpeningId",
                table: "EmploymentContracts",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CandidateProfileJson",
                table: "EmploymentApplications",
                type: "mediumtext",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<int>(
                name: "OfferedOpeningRevision",
                table: "EmploymentApplications",
                type: "int(11)",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevisionNumber",
                table: "EmploymentJobOpenings");

            migrationBuilder.DropColumn(
                name: "OriginApplicationId",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "OriginOpeningId",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "CandidateProfileJson",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "OfferedOpeningRevision",
                table: "EmploymentApplications");
        }
    }
}
