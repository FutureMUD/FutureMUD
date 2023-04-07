using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class IndexFixForBodyparts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "FK_BodyProtosPositions_BodyProtos_idx",
                table: "BodyProtosPositions",
                column: "BodyProtoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "FK_BodyProtosPositions_BodyProtos_idx",
                table: "BodyProtosPositions");
        }
    }
}
