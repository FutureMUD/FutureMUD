using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddGameItemProtoRefrigerationSensitivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "RefrigerationSensitive",
                table: "GameItemProtos",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefrigerationSensitive",
                table: "GameItemProtos");
        }
    }
}
