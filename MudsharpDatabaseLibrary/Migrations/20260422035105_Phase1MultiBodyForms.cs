using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class Phase1MultiBodyForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerScope",
                table: "TraitDefinitions",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.AddColumn<int>(
                name: "DominantHandAlignment",
                table: "Bodies",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'3'");

            migrationBuilder.CreateTable(
                name: "CharacterBodies",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Alias = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    SortOrder = table.Column<int>(type: "int(11)", nullable: false),
                    AllowVoluntarySwitch = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    CanVoluntarilySwitchProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WhyCannotVoluntarilySwitchProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.BodyId });
                    table.ForeignKey(
                        name: "FK_CharacterBodies_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterBodies_CanVoluntarilySwitchProg",
                        column: x => x.CanVoluntarilySwitchProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CharacterBodies_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterBodies_WhyCannotVoluntarilySwitchProg",
                        column: x => x.WhyCannotVoluntarilySwitchProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterTraits",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Value = table.Column<double>(type: "double", nullable: false),
                    AdditionalValue = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.TraitDefinitionId });
                    table.ForeignKey(
                        name: "FK_CharacterTraits_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterTraits_TraitDefinitions",
                        column: x => x.TraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterBodies_Bodies_idx",
                table: "CharacterBodies",
                column: "BodyId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterBodies_CanVoluntarilySwitchProg_idx",
                table: "CharacterBodies",
                column: "CanVoluntarilySwitchProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterBodies_WhyCannotVoluntarilySwitchProg_idx",
                table: "CharacterBodies",
                column: "WhyCannotVoluntarilySwitchProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterTraits_TraitDefinitions",
                table: "CharacterTraits",
                column: "TraitDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterBodies");

            migrationBuilder.DropTable(
                name: "CharacterTraits");

            migrationBuilder.DropColumn(
                name: "OwnerScope",
                table: "TraitDefinitions");

            migrationBuilder.DropColumn(
                name: "DominantHandAlignment",
                table: "Bodies");
        }
    }
}
