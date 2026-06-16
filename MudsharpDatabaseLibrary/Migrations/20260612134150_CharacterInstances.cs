using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CharacterInstances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    InstanceName = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    InstanceKind = table.Column<int>(type: "int(11)", nullable: false),
                    ControlPolicy = table.Column<int>(type: "int(11)", nullable: false),
                    DeathPolicy = table.Column<int>(type: "int(11)", nullable: false),
                    PerceptionPolicy = table.Column<int>(type: "int(11)", nullable: false),
                    PersistencePolicy = table.Column<int>(type: "int(11)", nullable: false),
                    LocationId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    PositionId = table.Column<int>(type: "int(11)", nullable: false),
                    PositionModifier = table.Column<int>(type: "int(11)", nullable: false),
                    PositionTargetId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PositionTargetType = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PositionEmote = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    State = table.Column<int>(type: "int(11)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    IsPrimary = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    IsEmbodied = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    IsControllable = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    PrimaryCharacterId = table.Column<long>(type: "bigint(20)", nullable: true, computedColumnSql: "CASE WHEN `IsPrimary` = b'1' THEN `CharacterId` ELSE NULL END", stored: true),
                    EmbodiedBodyId = table.Column<long>(type: "bigint(20)", nullable: true, computedColumnSql: "CASE WHEN `IsEmbodied` = b'1' THEN `BodyId` ELSE NULL END", stored: true),
                    AnchorInstanceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedBySourceType = table.Column<int>(type: "int(11)", nullable: true),
                    CreatedBySourceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedBySourceKey = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpiryDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    EffectData = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    // Legacy Characters.BodyId and Characters.Location were compatibility mirrors rather than
                    // enforced FKs. Keep the first-upgrade table tolerant and rely on diagnostics plus uniqueness
                    // guards so stale legacy mirrors do not block boot on pre-branch databases.
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterInstances_AnchorInstance_idx",
                table: "CharacterInstances",
                column: "AnchorInstanceId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterInstances_Bodies_idx",
                table: "CharacterInstances",
                column: "BodyId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterInstances_Cells_idx",
                table: "CharacterInstances",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterInstances_Characters_idx",
                table: "CharacterInstances",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInstances_Location_Layer",
                table: "CharacterInstances",
                columns: new[] { "LocationId", "RoomLayer" });

            migrationBuilder.CreateIndex(
                name: "UQ_CharacterInstances_EmbodiedBody",
                table: "CharacterInstances",
                column: "EmbodiedBodyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_CharacterInstances_PrimaryCharacter",
                table: "CharacterInstances",
                column: "PrimaryCharacterId",
                unique: true);

            migrationBuilder.Sql(
                @"INSERT INTO `CharacterInstances`
                    (`CharacterId`, `BodyId`, `InstanceName`, `InstanceKind`, `ControlPolicy`, `DeathPolicy`, `PerceptionPolicy`, `PersistencePolicy`,
                     `LocationId`, `RoomLayer`, `PositionId`, `PositionModifier`, `PositionTargetId`, `PositionTargetType`, `PositionEmote`,
                     `State`, `Status`, `IsPrimary`, `IsEmbodied`, `IsControllable`, `AnchorInstanceId`, `CreatedBySourceType`,
                     `CreatedBySourceId`, `CreatedBySourceKey`, `CreatedDateTime`, `ExpiryDateTime`, `EffectData`)
                  SELECT
                    c.`Id`,
                    c.`BodyId`,
                    NULL,
                    0,
                    CASE WHEN EXISTS (SELECT 1 FROM `NPCs` npc WHERE npc.`CharacterId` = c.`Id`) THEN 3 ELSE 1 END,
                    0,
                    0,
                    0,
                    CASE
                        WHEN EXISTS (
                            SELECT 1
                            FROM `Cells` cell
                            WHERE cell.`Id` = c.`Location`
                        )
                        THEN c.`Location`
                        ELSE NULL
                    END,
                    c.`RoomLayer`,
                    c.`PositionId`,
                    c.`PositionModifier`,
                    c.`PositionTargetId`,
                    c.`PositionTargetType`,
                    c.`PositionEmote`,
                    c.`State`,
                    c.`Status`,
                    b'1',
                    b'1',
                    b'1',
                    NULL,
                    NULL,
                    NULL,
                    NULL,
                    c.`CreationTime`,
                    NULL,
                    '<Effects/>'
                  FROM `Characters` c
                  WHERE NOT EXISTS (
                    SELECT 1
                    FROM `CharacterInstances` ci
                    WHERE ci.`CharacterId` = c.`Id`
                  );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterInstances");
        }
    }
}
