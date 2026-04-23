using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class Phase15BodyFormProvisioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CanSeeFormProgId",
                table: "CharacterBodies",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CharacterBodySources",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SourceType = table.Column<int>(type: "int(11)", nullable: false),
                    SourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SourceKey = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.SourceType, x.SourceId, x.SourceKey });
                    table.ForeignKey(
                        name: "FK_CharacterBodySources_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterBodySources_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterBodies_CanSeeFormProg_idx",
                table: "CharacterBodies",
                column: "CanSeeFormProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterBodySources_Bodies_idx",
                table: "CharacterBodySources",
                column: "BodyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterBodies_CanSeeFormProg",
                table: "CharacterBodies",
                column: "CanSeeFormProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterBodies_CanSeeFormProg",
                table: "CharacterBodies");

            migrationBuilder.DropTable(
                name: "CharacterBodySources");

            migrationBuilder.DropIndex(
                name: "FK_CharacterBodies_CanSeeFormProg_idx",
                table: "CharacterBodies");

            migrationBuilder.DropColumn(
                name: "CanSeeFormProgId",
                table: "CharacterBodies");
        }
    }
}
