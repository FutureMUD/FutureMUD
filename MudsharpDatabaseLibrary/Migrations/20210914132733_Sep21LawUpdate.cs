using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class Sep21LawUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "DoNotAutomaticallyApplyRepeats",
                table: "Laws",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoNotAutomaticallyApplyRepeats",
                table: "Laws");
        }
    }
}
