using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MerchandiseTransactionRecordsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopTransactionRecords_Merchandises_MerchandiseId",
                table: "ShopTransactionRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopTransactionRecords_Merchandises",
                table: "ShopTransactionRecords",
                column: "MerchandiseId",
                principalTable: "Merchandises",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopTransactionRecords_Merchandises",
                table: "ShopTransactionRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopTransactionRecords_Merchandises_MerchandiseId",
                table: "ShopTransactionRecords",
                column: "MerchandiseId",
                principalTable: "Merchandises",
                principalColumn: "Id");
        }
    }
}
