using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ItemProtoUniqueNameBuilderNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuilderNotes",
                table: "GameItemProtos",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "UniqueName",
                table: "GameItemProtos",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateIndex(
                name: "IX_GameItemProtos_UniqueName",
                table: "GameItemProtos",
                column: "UniqueName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameItemProtos_UniqueName",
                table: "GameItemProtos");

            migrationBuilder.DropColumn(
                name: "BuilderNotes",
                table: "GameItemProtos");

            migrationBuilder.DropColumn(
                name: "UniqueName",
                table: "GameItemProtos");
        }
    }
}
