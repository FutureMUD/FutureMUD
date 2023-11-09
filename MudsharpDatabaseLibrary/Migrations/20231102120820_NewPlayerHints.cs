using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class NewPlayerHints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "HintsEnabled",
                table: "Accounts",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");

            migrationBuilder.CreateTable(
                name: "NewPlayerHints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    FilterProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Priority = table.Column<int>(type: "int(11)", nullable: false),
                    CanRepeat = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewPlayerHints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewPlayerHints_FutureProgs",
                        column: x => x.FilterProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_NewPlayerHints_FilterProgId",
                table: "NewPlayerHints",
                column: "FilterProgId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewPlayerHints");

            migrationBuilder.DropColumn(
                name: "HintsEnabled",
                table: "Accounts");
        }
    }
}
