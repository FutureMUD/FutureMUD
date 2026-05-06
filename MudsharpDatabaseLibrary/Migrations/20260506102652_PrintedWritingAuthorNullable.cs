using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class PrintedWritingAuthorNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Writings_Characters_Author",
                table: "Writings");

            migrationBuilder.AlterColumn<long>(
                name: "AuthorId",
                table: "Writings",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AddForeignKey(
                name: "FK_Writings_Characters_Author",
                table: "Writings",
                column: "AuthorId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Writings_Characters_Author",
                table: "Writings");

            migrationBuilder.AlterColumn<long>(
                name: "AuthorId",
                table: "Writings",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Writings_Characters_Author",
                table: "Writings",
                column: "AuthorId",
                principalTable: "Characters",
                principalColumn: "Id");
        }
    }
}
