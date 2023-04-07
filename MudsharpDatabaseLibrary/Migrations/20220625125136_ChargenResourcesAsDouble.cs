using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class ChargenResourcesAsDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MaximumNumberAwardedPerAward",
                table: "ChargenResources",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MaximumNumberAwardedPerAward",
                table: "ChargenResources",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");
        }
    }
}
