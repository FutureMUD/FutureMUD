using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
	public partial class HospitalClinicalPlanning : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "OfferingMode",
				table: "HospitalServices",
				type: "int(11)",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<string>(
				name: "ProcedureParameters",
				table: "HospitalServiceRequests",
				type: "mediumtext",
				nullable: false,
				defaultValue: "")
				.Annotation("MySql:CharSet", "utf8")
				.Annotation("Relational:Collation", "utf8_general_ci");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "OfferingMode",
				table: "HospitalServices");

			migrationBuilder.DropColumn(
				name: "ProcedureParameters",
				table: "HospitalServiceRequests");
		}
	}
}
