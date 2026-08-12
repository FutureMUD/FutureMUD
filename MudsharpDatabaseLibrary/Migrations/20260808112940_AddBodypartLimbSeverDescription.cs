using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddBodypartLimbSeverDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "UseLimbSeverDescription",
                table: "BodypartProto",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseLimbSeverDescription",
                table: "BodypartProto");
        }
    }
}
