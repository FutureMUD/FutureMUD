using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class NameCulturesGenderExpansion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveProjects_Characters",
                table: "ActiveProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Cultures_NameCulture",
                table: "Cultures");

            migrationBuilder.DropIndex(
                name: "FK_Cultures_NameCulture",
                table: "Cultures");

            migrationBuilder.CreateTable(
                name: "CulturesNameCultures",
                columns: table => new
                {
                    CultureId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NameCultureId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Gender = table.Column<short>(type: "smallint(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CultureId, x.NameCultureId, x.Gender });
                    table.ForeignKey(
                        name: "FK_CulturesNameCultures_Cultures",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CulturesNameCultures_NameCultures",
                        column: x => x.NameCultureId,
                        principalTable: "NameCulture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CulturesNameCultures_NameCultureId",
                table: "CulturesNameCultures",
                column: "NameCultureId");

            migrationBuilder.Sql(@"INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 0
FROM Cultures");
            migrationBuilder.Sql(@"INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 1
FROM Cultures");
            migrationBuilder.Sql(@"INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 2
FROM Cultures");
            migrationBuilder.Sql(@"INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 3
FROM Cultures");
            migrationBuilder.Sql(@"INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 4
FROM Cultures");

			migrationBuilder.AddForeignKey(
                name: "FK_ActiveProjects_Characters",
                table: "ActiveProjects",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropColumn(
	            name: "NameCultureId",
	            table: "Cultures");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveProjects_Characters",
                table: "ActiveProjects");

            migrationBuilder.DropTable(
                name: "CulturesNameCultures");

            migrationBuilder.AddColumn<long>(
                name: "NameCultureId",
                table: "Cultures",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "FK_Cultures_NameCulture",
                table: "Cultures",
                column: "NameCultureId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveProjects_Characters",
                table: "ActiveProjects",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cultures_NameCulture",
                table: "Cultures",
                column: "NameCultureId",
                principalTable: "NameCulture",
                principalColumn: "Id");
        }
    }
}
