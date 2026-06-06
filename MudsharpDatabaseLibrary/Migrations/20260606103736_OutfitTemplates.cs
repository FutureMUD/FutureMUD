using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class OutfitTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutfitTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Exclusivity = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OutfitTemplateItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OutfitTemplateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TemplateKey = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    GameItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WearProfileId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Placement = table.Column<int>(type: "int(11)", nullable: false),
                    ContainerKey = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LoadArguments = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    WearOrder = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutfitTemplateItems_OutfitTemplates",
                        column: x => x.OutfitTemplateId,
                        principalTable: "OutfitTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutfitTemplateItems_WearProfiles",
                        column: x => x.WearProfileId,
                        principalTable: "WearProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_OutfitTemplateItems_OutfitTemplates_idx",
                table: "OutfitTemplateItems",
                column: "OutfitTemplateId");

            migrationBuilder.CreateIndex(
                name: "FK_OutfitTemplateItems_WearProfiles_idx",
                table: "OutfitTemplateItems",
                column: "WearProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OutfitTemplateItems_GameItemProtoId",
                table: "OutfitTemplateItems",
                column: "GameItemProtoId");

            migrationBuilder.CreateIndex(
                name: "IX_OutfitTemplateItems_Template_Key",
                table: "OutfitTemplateItems",
                columns: new[] { "OutfitTemplateId", "TemplateKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutfitTemplates_Name",
                table: "OutfitTemplates",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutfitTemplateItems");

            migrationBuilder.DropTable(
                name: "OutfitTemplates");
        }
    }
}
