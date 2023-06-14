using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class CraftUseToolDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "UseToolDuration",
                table: "CraftTools",
                type: "bit(1)",
                nullable: false,
                defaultValue: 1ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseToolDuration",
                table: "CraftTools");
        }
    }
}
