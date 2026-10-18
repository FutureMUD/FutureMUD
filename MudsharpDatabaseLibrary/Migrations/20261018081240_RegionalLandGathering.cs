using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RegionalLandGathering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegionalGatheringProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Revision = table.Column<long>(type: "bigint", nullable: false),
                    Definition = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RegionalGatheringPools",
                columns: table => new
                {
                    OwnerKind = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProfileId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Revision = table.Column<long>(type: "bigint", nullable: false),
                    Stock = table.Column<double>(type: "double", nullable: false),
                    Capacity = table.Column<double>(type: "double", nullable: false),
                    Initialised = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Inherit = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.OwnerKind, x.OwnerId });
                    table.ForeignKey(
                        name: "FK_RegionalGatheringPools_RegionalGatheringProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "RegionalGatheringProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MagicGatheringParticipants_SharedSource",
                table: "MagicGatheringParticipants",
                column: "SourceKey");

            migrationBuilder.CreateIndex(
                name: "IX_RegionalGatheringPools_ProfileId",
                table: "RegionalGatheringPools",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionalGatheringPools");

            migrationBuilder.DropTable(
                name: "RegionalGatheringProfiles");

            migrationBuilder.DropIndex(
                name: "IX_MagicGatheringParticipants_SharedSource",
                table: "MagicGatheringParticipants");
        }
    }
}
