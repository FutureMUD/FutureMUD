using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class Skins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SkinId",
                table: "GameItems",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "PermitPlayerSkins",
                table: "GameItemProtos",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");

            migrationBuilder.CreateTable(
                name: "GameItemSkins",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ItemName = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShortDescription = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullDescription = table.Column<string>(type: "varchar(2000)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LongDescription = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quality = table.Column<int>(type: "int(11)", nullable: true),
                    IsPublic = table.Column<ulong>(type: "bit(1)", nullable: false),
                    CanUseSkinProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_GameItemSkins_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GameItemSkins_EditableItemId",
                table: "GameItemSkins",
                column: "EditableItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameItemSkins");

            migrationBuilder.DropColumn(
                name: "SkinId",
                table: "GameItems");

            migrationBuilder.DropColumn(
                name: "PermitPlayerSkins",
                table: "GameItemProtos");
        }
    }
}
