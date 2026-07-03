using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class WritingCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WritingCollections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DefaultTitle = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WritingCollectionEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WritingCollectionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PageNumber = table.Column<int>(type: "int(11)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false),
                    WritingId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DrawingId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WritingCollectionEntries_Collections",
                        column: x => x.WritingCollectionId,
                        principalTable: "WritingCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WritingCollectionEntries_Drawings",
                        column: x => x.DrawingId,
                        principalTable: "Drawings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WritingCollectionEntries_Writings",
                        column: x => x.WritingId,
                        principalTable: "Writings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_WritingCollectionEntries_Collections_idx",
                table: "WritingCollectionEntries",
                column: "WritingCollectionId");

            migrationBuilder.CreateIndex(
                name: "FK_WritingCollectionEntries_Drawings_idx",
                table: "WritingCollectionEntries",
                column: "DrawingId");

            migrationBuilder.CreateIndex(
                name: "FK_WritingCollectionEntries_Writings_idx",
                table: "WritingCollectionEntries",
                column: "WritingId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingCollectionEntries_Page_Order",
                table: "WritingCollectionEntries",
                columns: new[] { "WritingCollectionId", "PageNumber", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_WritingCollections_Name",
                table: "WritingCollections",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WritingCollectionEntries");

            migrationBuilder.DropTable(
                name: "WritingCollections");
        }
    }
}
