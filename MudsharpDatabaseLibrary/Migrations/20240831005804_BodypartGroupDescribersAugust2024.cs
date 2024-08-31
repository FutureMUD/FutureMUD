using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class BodypartGroupDescribersAugust2024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BodyProtoId",
                table: "BodypartGroupDescribers",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BodypartGroupDescribers_BodyProtoId",
                table: "BodypartGroupDescribers",
                column: "BodyProtoId");

            migrationBuilder.AddForeignKey(
                name: "FK_BodypartGroupDescribers_BodyProtos_BodyProtoId",
                table: "BodypartGroupDescribers",
                column: "BodyProtoId",
                principalTable: "BodyProtos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BodypartGroupDescribers_BodyProtos_BodyProtoId",
                table: "BodypartGroupDescribers");

            migrationBuilder.DropIndex(
                name: "IX_BodypartGroupDescribers_BodyProtoId",
                table: "BodypartGroupDescribers");

            migrationBuilder.DropColumn(
                name: "BodyProtoId",
                table: "BodypartGroupDescribers");
        }
    }
}
