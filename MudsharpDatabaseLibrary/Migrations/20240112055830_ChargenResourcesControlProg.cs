using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ChargenResourcesControlProg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ControlProgId",
                table: "ChargenResources",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FK_ChargenResources_FutureProgs",
                table: "ChargenResources",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Accounts_ChargenResources",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.CreateIndex(
                name: "IX_ChargenResources_FK_ChargenResources_FutureProgs",
                table: "ChargenResources",
                column: "FK_ChargenResources_FutureProgs");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargenResources_FutureProgs_FK_ChargenResources_FutureProgs",
                table: "ChargenResources",
                column: "FK_ChargenResources_FutureProgs",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargenResources_FutureProgs_FK_ChargenResources_FutureProgs",
                table: "ChargenResources");

            migrationBuilder.DropIndex(
                name: "IX_ChargenResources_FK_ChargenResources_FutureProgs",
                table: "ChargenResources");

            migrationBuilder.DropColumn(
                name: "ControlProgId",
                table: "ChargenResources");

            migrationBuilder.DropColumn(
                name: "FK_ChargenResources_FutureProgs",
                table: "ChargenResources");

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Accounts_ChargenResources",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");
        }
    }
}
