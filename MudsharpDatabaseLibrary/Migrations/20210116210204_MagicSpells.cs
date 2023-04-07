using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class MagicSpells : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MagicSpells",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Blurb = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    SpellKnownProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MagicSchoolId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ExclusiveDelay = table.Column<double>(type: "double", nullable: false),
                    NonExclusiveDelay = table.Column<double>(type: "double", nullable: false),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicSpells_Futureprogs",
                        column: x => x.SpellKnownProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MagicSpells_MagicSchools",
                        column: x => x.MagicSchoolId,
                        principalTable: "MagicSchools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "FK_MagicSpells_Futureprogs_idx",
                table: "MagicSpells",
                column: "SpellKnownProgId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicSpells_MagicSchools_idx",
                table: "MagicSpells",
                column: "MagicSchoolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MagicSpells");
        }
    }
}
