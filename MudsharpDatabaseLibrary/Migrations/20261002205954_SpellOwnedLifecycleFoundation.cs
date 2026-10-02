using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class SpellOwnedLifecycleFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MagicSpellLifecycles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SpellId = table.Column<long>(type: "bigint", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    Family = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeadlineUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Provenance = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: true),
                    DeathObservedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RemainsItemId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Diagnostic = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicSpellLifecycles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MagicSpellOwnedEntities",
                columns: table => new
                {
                    Kind = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    LifecycleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicSpellOwnedEntities", x => new { x.Kind, x.EntityId });
                    table.ForeignKey(
                        name: "FK_MagicSpellOwnedEntities_MagicSpellLifecycles_LifecycleId",
                        column: x => x.LifecycleId,
                        principalTable: "MagicSpellLifecycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MagicSpellLifecycles_CreatorId_Family_State",
                table: "MagicSpellLifecycles",
                columns: new[] { "CreatorId", "Family", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_MagicSpellLifecycles_State_DeadlineUtc",
                table: "MagicSpellLifecycles",
                columns: new[] { "State", "DeadlineUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MagicSpellOwnedEntities_LifecycleId",
                table: "MagicSpellOwnedEntities",
                column: "LifecycleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MagicSpellOwnedEntities");

            migrationBuilder.DropTable(
                name: "MagicSpellLifecycles");
        }
    }
}
