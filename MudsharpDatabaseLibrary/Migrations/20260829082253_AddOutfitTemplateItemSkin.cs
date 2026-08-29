using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddOutfitTemplateItemSkin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SkinId",
                table: "OutfitTemplateItems",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutfitTemplateItems_SkinId",
                table: "OutfitTemplateItems",
                column: "SkinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutfitTemplateItems_SkinId",
                table: "OutfitTemplateItems");

            migrationBuilder.DropColumn(
                name: "SkinId",
                table: "OutfitTemplateItems");
        }
    }
}
