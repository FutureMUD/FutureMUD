using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class PropertyV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BondClaimed",
                table: "PropertyLeases",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "PropertyKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PropertyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AddedToPropertyOnDate = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CostToReplace = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    IsReturned = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyKeys_GameItems",
                        column: x => x.GameItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyKeys_Property",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyKeys_GameItemId",
                table: "PropertyKeys",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyKeys_PropertyId",
                table: "PropertyKeys",
                column: "PropertyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyKeys");

            migrationBuilder.DropColumn(
                name: "BondClaimed",
                table: "PropertyLeases");
        }
    }
}
