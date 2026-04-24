using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class PlanesAndPlanarData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlanarData",
                table: "GameItemProtos",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "PlanarData",
                table: "BodyProtos",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "Planes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Alias = table.Column<string>(type: "varchar(500)", nullable: false, defaultValueSql: "''", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false),
                    IsDefault = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Planes");

            migrationBuilder.DropColumn(
                name: "PlanarData",
                table: "GameItemProtos");

            migrationBuilder.DropColumn(
                name: "PlanarData",
                table: "BodyProtos");
        }
    }
}
