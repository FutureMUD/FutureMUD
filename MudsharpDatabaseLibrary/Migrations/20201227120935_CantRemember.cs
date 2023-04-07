using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class CantRemember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyProtosPositions",
                columns: table => new
                {
                    BodyProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Position = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyProtosPositions", x => new { x.BodyProtoId, x.Position });
                    table.ForeignKey(
                        name: "FK_BodyProtosPositions_BodyProtos",
                        column: x => x.BodyProtoId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyProtosPositions");
        }
    }
}
