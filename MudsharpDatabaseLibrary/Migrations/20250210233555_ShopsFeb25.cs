using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ShopsFeb25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MerchandiseId",
                table: "ShopTransactionRecords",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalesMarkupMultiplier",
                table: "Merchandises",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopTransactionRecords_MerchandiseId",
                table: "ShopTransactionRecords",
                column: "MerchandiseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopTransactionRecords_Merchandises_MerchandiseId",
                table: "ShopTransactionRecords",
                column: "MerchandiseId",
                principalTable: "Merchandises",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopTransactionRecords_Merchandises_MerchandiseId",
                table: "ShopTransactionRecords");

            migrationBuilder.DropIndex(
                name: "IX_ShopTransactionRecords_MerchandiseId",
                table: "ShopTransactionRecords");

            migrationBuilder.DropColumn(
                name: "MerchandiseId",
                table: "ShopTransactionRecords");

            migrationBuilder.DropColumn(
                name: "SalesMarkupMultiplier",
                table: "Merchandises");
        }
    }
}
