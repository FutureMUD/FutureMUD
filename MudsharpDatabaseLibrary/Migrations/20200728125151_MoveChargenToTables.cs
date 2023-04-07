using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class MoveChargenToTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChargenScreenStoryboards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChargenType = table.Column<string>(type: "varchar(50)", nullable: true),
                    ChargenStage = table.Column<int>(type: "int(11)", nullable: false),
                    Order = table.Column<int>(type: "int(11)", nullable: false),
                    StageDefinition = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    NextStage = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargenScreenStoryboards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargenScreenStoryboardDependentStages",
                columns: table => new
                {
                    OwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Dependency = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.OwnerId, x.Dependency });
                    table.ForeignKey(
                        name: "FK_ChargenScreenStoryboardDependentStages_Owner",
                        column: x => x.OwnerId,
                        principalTable: "ChargenScreenStoryboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "FK_ChargenScreenStoryboardDependentStages_Owner",
                table: "ChargenScreenStoryboardDependentStages",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargenScreenStoryboardDependentStages");

            migrationBuilder.DropTable(
                name: "ChargenScreenStoryboards");
        }
    }
}
