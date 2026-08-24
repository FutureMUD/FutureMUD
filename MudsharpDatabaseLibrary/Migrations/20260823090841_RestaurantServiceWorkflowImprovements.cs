using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantServiceWorkflowImprovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TakeawayBagPrototypeId",
                table: "RestaurantMenuItems");

            migrationBuilder.DropColumn(
                name: "TakeawayBagPrototypeRevisionNumber",
                table: "RestaurantMenuItems");

            migrationBuilder.AddColumn<long>(
                name: "TakeawayBagPrototypeId",
                table: "Restaurants",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TakeawayBagPrototypeRevisionNumber",
                table: "Restaurants",
                type: "int(11)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RestaurantStorageContainers",
                columns: table => new
                {
                    RestaurantShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Roles = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RestaurantShopId, x.GameItemId });
                    table.ForeignKey(
                        name: "FK_RestaurantStorageContainers_Restaurants",
                        column: x => x.RestaurantShopId,
                        principalTable: "Restaurants",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantStorageContainers_GameItem",
                table: "RestaurantStorageContainers",
                column: "GameItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantStorageContainers");

            migrationBuilder.DropColumn(
                name: "TakeawayBagPrototypeId",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "TakeawayBagPrototypeRevisionNumber",
                table: "Restaurants");

            migrationBuilder.AddColumn<long>(
                name: "TakeawayBagPrototypeId",
                table: "RestaurantMenuItems",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TakeawayBagPrototypeRevisionNumber",
                table: "RestaurantMenuItems",
                type: "int(11)",
                nullable: true);
        }
    }
}
