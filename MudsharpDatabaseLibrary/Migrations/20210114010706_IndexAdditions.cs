using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class IndexAdditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "FK_BodyProtos_AdditionalBodyparts_BodypartProto",
                table: "BodyProtos_AdditionalBodyparts",
                newName: "FK_BodyProtos_AdditionalBodyparts_BodypartProto_idx");

            migrationBuilder.CreateIndex(
                name: "FK_BodyProtos_AdditionalBodyparts_BodyProtos_idx",
                table: "BodyProtos_AdditionalBodyparts",
                column: "BodyProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_BodypartInternalInfos_BodypartProtos_idx",
                table: "BodypartInternalInfos",
                column: "BodypartProtoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "FK_BodyProtos_AdditionalBodyparts_BodyProtos_idx",
                table: "BodyProtos_AdditionalBodyparts");

            migrationBuilder.DropIndex(
                name: "FK_BodypartInternalInfos_BodypartProtos_idx",
                table: "BodypartInternalInfos");

            migrationBuilder.RenameIndex(
                name: "FK_BodyProtos_AdditionalBodyparts_BodypartProto_idx",
                table: "BodyProtos_AdditionalBodyparts",
                newName: "FK_BodyProtos_AdditionalBodyparts_BodypartProto");
        }
    }
}
