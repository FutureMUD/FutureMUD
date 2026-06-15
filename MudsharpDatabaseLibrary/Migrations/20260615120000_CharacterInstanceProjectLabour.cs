using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Database;

#nullable disable

namespace MudSharp.Migrations
{
	/// <inheritdoc />
	[DbContext(typeof(FuturemudDatabaseContext))]
	[Migration("20260615120000_CharacterInstanceProjectLabour")]
	public partial class CharacterInstanceProjectLabour : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<long>(
				name: "CurrentProjectId",
				table: "CharacterInstances",
				type: "bigint(20)",
				nullable: true);

			migrationBuilder.AddColumn<long>(
				name: "CurrentProjectLabourId",
				table: "CharacterInstances",
				type: "bigint(20)",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				name: "CurrentProjectHours",
				table: "CharacterInstances",
				type: "double",
				nullable: false,
				defaultValue: 0.0);

			migrationBuilder.AddColumn<double>(
				name: "CurrentProjectProjectHours",
				table: "CharacterInstances",
				type: "double",
				nullable: false,
				defaultValue: 0.0);

			migrationBuilder.CreateIndex(
				name: "FK_CharacterInstances_ActiveProjects_idx",
				table: "CharacterInstances",
				column: "CurrentProjectId");

			migrationBuilder.CreateIndex(
				name: "FK_CharacterInstances_ProjectLabourRequirements_idx",
				table: "CharacterInstances",
				column: "CurrentProjectLabourId");

			migrationBuilder.Sql(
				@"UPDATE `CharacterInstances` ci
				  INNER JOIN `Characters` c ON c.`Id` = ci.`CharacterId`
				  SET
					ci.`CurrentProjectId` = c.`CurrentProjectId`,
					ci.`CurrentProjectLabourId` = c.`CurrentProjectLabourId`,
					ci.`CurrentProjectHours` = c.`CurrentProjectHours`,
					ci.`CurrentProjectProjectHours` = c.`CurrentProjectProjectHours`
				  WHERE ci.`IsPrimary` = b'1';");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "FK_CharacterInstances_ActiveProjects_idx",
				table: "CharacterInstances");

			migrationBuilder.DropIndex(
				name: "FK_CharacterInstances_ProjectLabourRequirements_idx",
				table: "CharacterInstances");

			migrationBuilder.DropColumn(
				name: "CurrentProjectId",
				table: "CharacterInstances");

			migrationBuilder.DropColumn(
				name: "CurrentProjectLabourId",
				table: "CharacterInstances");

			migrationBuilder.DropColumn(
				name: "CurrentProjectHours",
				table: "CharacterInstances");

			migrationBuilder.DropColumn(
				name: "CurrentProjectProjectHours",
				table: "CharacterInstances");
		}
	}
}
