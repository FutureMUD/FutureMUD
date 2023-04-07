using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class ExtraDescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameItemProtoExtraDescriptions",
                columns: table => new
                {
                    GameItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemProtoRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    ApplicabilityProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ShortDescription = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullDescription = table.Column<string>(type: "varchar(2000)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullDescriptionAddendum = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Priority = table.Column<int>(type: "int(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.GameItemProtoId, x.GameItemProtoRevisionNumber, x.ApplicabilityProgId });
                    table.ForeignKey(
                        name: "FK_GameItemProtoExtraDescriptions_FutureProgs",
                        column: x => x.ApplicabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameItemProtoExtraDescriptions_GameItemProtos",
                        columns: x => new { x.GameItemProtoId, x.GameItemProtoRevisionNumber },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameItemProtoExtraDescriptions_ApplicabilityProgId",
                table: "GameItemProtoExtraDescriptions",
                column: "ApplicabilityProgId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameItemProtoExtraDescriptions");
        }
    }
}
