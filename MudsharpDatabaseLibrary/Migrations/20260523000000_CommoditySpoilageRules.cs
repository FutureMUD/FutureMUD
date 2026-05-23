using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
	/// <inheritdoc />
	public partial class CommoditySpoilageRules : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "CommoditySpoilageRules",
				columns: table => new
				{
					Id = table.Column<long>(type: "bigint(20)", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					Name = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8mb4_unicode_ci")
						.Annotation("MySql:CharSet", "utf8mb4"),
					Description = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
						.Annotation("MySql:CharSet", "utf8mb4"),
					Enabled = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
					Priority = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
					MaterialId = table.Column<long>(type: "bigint(20)", nullable: true),
					MaterialTagId = table.Column<long>(type: "bigint(20)", nullable: true),
					CommodityTagId = table.Column<long>(type: "bigint(20)", nullable: true),
					ResultMaterialId = table.Column<long>(type: "bigint(20)", nullable: false),
					ResultCommodityTagId = table.Column<long>(type: "bigint(20)", nullable: true),
					SecondsUntilSpoiled = table.Column<long>(type: "bigint(20)", nullable: false),
					SpoilEcho = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PRIMARY", x => x.Id);
					table.ForeignKey(
						name: "FK_CommoditySpoilageRules_CommodityTags",
						column: x => x.CommodityTagId,
						principalTable: "Tags",
						principalColumn: "Id",
						onDelete: ReferentialAction.SetNull);
					table.ForeignKey(
						name: "FK_CommoditySpoilageRules_MaterialTags",
						column: x => x.MaterialTagId,
						principalTable: "Tags",
						principalColumn: "Id",
						onDelete: ReferentialAction.SetNull);
					table.ForeignKey(
						name: "FK_CommoditySpoilageRules_Materials",
						column: x => x.MaterialId,
						principalTable: "Materials",
						principalColumn: "Id",
						onDelete: ReferentialAction.SetNull);
					table.ForeignKey(
						name: "FK_CommoditySpoilageRules_ResultMaterials",
						column: x => x.ResultMaterialId,
						principalTable: "Materials",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_CommoditySpoilageRules_ResultTags",
						column: x => x.ResultCommodityTagId,
						principalTable: "Tags",
						principalColumn: "Id",
						onDelete: ReferentialAction.SetNull);
				})
				.Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateIndex(
				name: "FK_CommoditySpoilageRules_CommodityTags_idx",
				table: "CommoditySpoilageRules",
				column: "CommodityTagId");

			migrationBuilder.CreateIndex(
				name: "FK_CommoditySpoilageRules_MaterialTags_idx",
				table: "CommoditySpoilageRules",
				column: "MaterialTagId");

			migrationBuilder.CreateIndex(
				name: "FK_CommoditySpoilageRules_Materials_idx",
				table: "CommoditySpoilageRules",
				column: "MaterialId");

			migrationBuilder.CreateIndex(
				name: "FK_CommoditySpoilageRules_ResultMaterials_idx",
				table: "CommoditySpoilageRules",
				column: "ResultMaterialId");

			migrationBuilder.CreateIndex(
				name: "FK_CommoditySpoilageRules_ResultTags_idx",
				table: "CommoditySpoilageRules",
				column: "ResultCommodityTagId");

			migrationBuilder.CreateIndex(
				name: "IX_CommoditySpoilageRules_Name",
				table: "CommoditySpoilageRules",
				column: "Name",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "CommoditySpoilageRules");
		}
	}
}
