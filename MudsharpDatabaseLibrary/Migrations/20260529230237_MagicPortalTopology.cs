using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MagicPortalTopology : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MagicPortalNetworks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    MagicSchoolId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsActive = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    AllowCrossZone = table.Column<ulong>(type: "bit(1)", nullable: false),
                    Verb = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    OutboundKeyword = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    InboundKeyword = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    OutboundTarget = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    InboundTarget = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    OutboundDescription = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    InboundDescription = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    TimeMultiplier = table.Column<double>(type: "double", nullable: false, defaultValue: 1.0),
                    CreatedByCharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedBySpellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicPortalNetworks_Characters",
                        column: x => x.CreatedByCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MagicPortalNetworks_MagicSchools",
                        column: x => x.MagicSchoolId,
                        principalTable: "MagicSchools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MagicPortalNetworks_MagicSpells",
                        column: x => x.CreatedBySpellId,
                        principalTable: "MagicSpells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MagicPortalEndpoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MagicPortalNetworkId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Key = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AnchorType = table.Column<int>(type: "int(11)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsActive = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    CreatedByCharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedBySpellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicPortalEndpoints_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MagicPortalEndpoints_Characters",
                        column: x => x.CreatedByCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MagicPortalEndpoints_GameItems",
                        column: x => x.GameItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MagicPortalEndpoints_MagicPortalNetworks",
                        column: x => x.MagicPortalNetworkId,
                        principalTable: "MagicPortalNetworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MagicPortalEndpoints_MagicSpells",
                        column: x => x.CreatedBySpellId,
                        principalTable: "MagicSpells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MagicPortalLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MagicPortalNetworkId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SourceEndpointId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DestinationEndpointId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsActive = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    CreatedByCharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedBySpellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicPortalLinks_Characters",
                        column: x => x.CreatedByCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MagicPortalLinks_DestinationEndpoints",
                        column: x => x.DestinationEndpointId,
                        principalTable: "MagicPortalEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MagicPortalLinks_MagicPortalNetworks",
                        column: x => x.MagicPortalNetworkId,
                        principalTable: "MagicPortalNetworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MagicPortalLinks_MagicSpells",
                        column: x => x.CreatedBySpellId,
                        principalTable: "MagicSpells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MagicPortalLinks_SourceEndpoints",
                        column: x => x.SourceEndpointId,
                        principalTable: "MagicPortalEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalEndpoints_Cells_idx",
                table: "MagicPortalEndpoints",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalEndpoints_Characters_idx",
                table: "MagicPortalEndpoints",
                column: "CreatedByCharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalEndpoints_GameItems_idx",
                table: "MagicPortalEndpoints",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalEndpoints_MagicSpells_idx",
                table: "MagicPortalEndpoints",
                column: "CreatedBySpellId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicPortalEndpoints_Network_Key",
                table: "MagicPortalEndpoints",
                columns: new[] { "MagicPortalNetworkId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalLinks_Characters_idx",
                table: "MagicPortalLinks",
                column: "CreatedByCharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalLinks_DestinationEndpoints_idx",
                table: "MagicPortalLinks",
                column: "DestinationEndpointId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalLinks_MagicSpells_idx",
                table: "MagicPortalLinks",
                column: "CreatedBySpellId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalLinks_SourceEndpoints_idx",
                table: "MagicPortalLinks",
                column: "SourceEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicPortalLinks_Network_Source_Destination",
                table: "MagicPortalLinks",
                columns: new[] { "MagicPortalNetworkId", "SourceEndpointId", "DestinationEndpointId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalNetworks_Characters_idx",
                table: "MagicPortalNetworks",
                column: "CreatedByCharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalNetworks_MagicSchools_idx",
                table: "MagicPortalNetworks",
                column: "MagicSchoolId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPortalNetworks_MagicSpells_idx",
                table: "MagicPortalNetworks",
                column: "CreatedBySpellId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MagicPortalLinks");

            migrationBuilder.DropTable(
                name: "MagicPortalEndpoints");

            migrationBuilder.DropTable(
                name: "MagicPortalNetworks");
        }
    }
}
