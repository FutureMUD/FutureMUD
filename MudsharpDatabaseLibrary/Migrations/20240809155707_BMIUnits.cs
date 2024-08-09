using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
	/// <inheritdoc />
	public partial class BMIUnits : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"INSERT INTO UnitOfMeasure (
`Name`,
`PrimaryAbbreviation`,
`Abbreviations`,
`BaseMultiplier`,
`PreMultiplierBaseOffset`,
`PostMultiplierBaseOffset`,
`Type`,
`Describer`,
`SpaceBetween`,
`System`,
`DefaultUnitForSystem`)
VALUES
(
	'kg/m²',
	'kg/m²',
	'kg kgm kgms',
	1.0,
	0,
	0,
	9,
	b'1',
	b'1',
	'Metric',
	b'1'
);");

			migrationBuilder.Sql(@"INSERT INTO UnitOfMeasure (
`Name`,
`PrimaryAbbreviation`,
`Abbreviations`,
`BaseMultiplier`,
`PreMultiplierBaseOffset`,
`PostMultiplierBaseOffset`,
`Type`,
`Describer`,
`SpaceBetween`,
`System`,
`DefaultUnitForSystem`)
VALUES
(
	'lb/in²',
	'lb/in²',
	'lb lbin lbin2 lbsqin',
	0.00142247510668563300142247510669,
	0,
	0,
	9,
	b'1',
	b'1',
	'Imperial',
	b'1'
);");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
