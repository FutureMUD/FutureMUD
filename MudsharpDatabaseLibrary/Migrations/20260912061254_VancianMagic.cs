using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VancianMagic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ScrollInscriptionAllowed",
                table: "MagicSpells",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SpellLevel",
                table: "MagicSpells",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CharacterMagicCapabilityStates",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MagicCapabilityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    StateVersion = table.Column<long>(type: "bigint", nullable: false),
                    Definition = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMagicCapabilityStates", x => new { x.CharacterId, x.MagicCapabilityId });
                    table.ForeignKey(
                        name: "FK_CharacterMagicCapabilityStates_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterMagicCapabilityStates_MagicCapabilities_MagicCapabi~",
                        column: x => x.MagicCapabilityId,
                        principalTable: "MagicCapabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VancianMagicOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CharacterId = table.Column<long>(type: "bigint", nullable: false),
                    MagicCapabilityId = table.Column<long>(type: "bigint", nullable: false),
                    SourceItemId = table.Column<long>(type: "bigint", nullable: true),
                    DestinationItemId = table.Column<long>(type: "bigint", nullable: true),
                    Kind = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpectedStateVersion = table.Column<long>(type: "bigint", nullable: false),
                    Definition = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Diagnostic = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VancianMagicOperations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMagicCapabilityStates_MagicCapabilityId",
                table: "CharacterMagicCapabilityStates",
                column: "MagicCapabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_VancianMagicOperations_CharacterId_MagicCapabilityId",
                table: "VancianMagicOperations",
                columns: new[] { "CharacterId", "MagicCapabilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_VancianMagicOperations_DestinationItemId",
                table: "VancianMagicOperations",
                column: "DestinationItemId");

            migrationBuilder.CreateIndex(
                name: "IX_VancianMagicOperations_SourceItemId",
                table: "VancianMagicOperations",
                column: "SourceItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterMagicCapabilityStates");

            migrationBuilder.DropTable(
                name: "VancianMagicOperations");

            migrationBuilder.DropColumn(
                name: "ScrollInscriptionAllowed",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "SpellLevel",
                table: "MagicSpells");
        }
    }
}
