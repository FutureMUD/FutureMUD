using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class HospitalServiceConsentPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			// HospitalServiceType.Stabilisation is persisted as 10.
            migrationBuilder.AddColumn<int>(
                name: "ConsentPolicy",
                table: "HospitalServices",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

			migrationBuilder.Sql("UPDATE `HospitalServices` SET `ConsentPolicy` = 1 WHERE `ServiceType` = 10;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentPolicy",
                table: "HospitalServices");
        }
    }
}
