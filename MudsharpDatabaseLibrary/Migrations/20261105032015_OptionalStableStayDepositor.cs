using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class OptionalStableStayDepositor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StableStays_Characters_Owner",
                table: "StableStays");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalOwnerName",
                table: "StableStays",
                type: "mediumtext",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<long>(
                name: "OriginalOwnerId",
                table: "StableStays",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AddForeignKey(
                name: "FK_StableStays_Characters_Owner",
                table: "StableStays",
                column: "OriginalOwnerId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StableStays_Characters_Owner",
                table: "StableStays");

            migrationBuilder.UpdateData(
                table: "StableStays",
                keyColumn: "OriginalOwnerName",
                keyValue: null,
                column: "OriginalOwnerName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalOwnerName",
                table: "StableStays",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<long>(
                name: "OriginalOwnerId",
                table: "StableStays",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StableStays_Characters_Owner",
                table: "StableStays",
                column: "OriginalOwnerId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
