using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class KnowledgeBuilding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnowledgesCosts",
                columns: table => new
                {
                    KnowledgeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Cost = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.KnowledgeId, x.ChargenResourceId });
                    table.ForeignKey(
                        name: "FK_KnowledgesCosts_ChargenResources",
                        column: x => x.ChargenResourceId,
                        principalTable: "ChargenResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnowledgesCosts_Knowledges",
                        column: x => x.KnowledgeId,
                        principalTable: "knowledges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "FK_KnowledgesCosts_ChargenResources_idx",
                table: "KnowledgesCosts",
                column: "ChargenResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_KnowledgesCosts_Knowledges_idx",
                table: "KnowledgesCosts",
                column: "KnowledgeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnowledgesCosts");
        }
    }
}
