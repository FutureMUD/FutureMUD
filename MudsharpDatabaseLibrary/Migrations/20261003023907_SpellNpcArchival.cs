using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class SpellNpcArchival : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Bodies",
                table: "Characters");

            migrationBuilder.AlterColumn<long>(
                name: "BodyId",
                table: "Characters",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Characters",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CharacterArchives",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OriginalBodyId = table.Column<long>(type: "bigint", nullable: false),
                    LifecycleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ArchivedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DisplayName = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShortDescription = table.Column<string>(type: "varchar(4096)", maxLength: 4096, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullDescription = table.Column<string>(type: "longtext", maxLength: 65535, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WoundHistory = table.Column<string>(type: "longtext", maxLength: 65535, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Provenance = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterArchives", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_CharacterArchives_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Characters_BodyOrArchive",
                table: "Characters",
                sql: "`BodyId` IS NOT NULL OR `IsArchived` = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterArchives_LifecycleId",
                table: "CharacterArchives",
                column: "LifecycleId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Bodies",
                table: "Characters",
                column: "BodyId",
                principalTable: "Bodies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Bodies",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "CharacterArchives");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Characters_BodyOrArchive",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Characters");

            migrationBuilder.AlterColumn<long>(
                name: "BodyId",
                table: "Characters",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Bodies",
                table: "Characters",
                column: "BodyId",
                principalTable: "Bodies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
