using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ArenaStageNameProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultStageNameTemplate",
                table: "ArenaCombatantClasses");

            migrationBuilder.AddColumn<long>(
                name: "DefaultStageNameProfileId",
                table: "ArenaCombatantClasses",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_ArenaCombatantClasses_DefaultStageNameProfile",
                table: "ArenaCombatantClasses",
                column: "DefaultStageNameProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArenaCombatantClasses_DefaultStageNameProfile",
                table: "ArenaCombatantClasses",
                column: "DefaultStageNameProfileId",
                principalTable: "RandomNameProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArenaCombatantClasses_DefaultStageNameProfile",
                table: "ArenaCombatantClasses");

            migrationBuilder.DropIndex(
                name: "FK_ArenaCombatantClasses_DefaultStageNameProfile",
                table: "ArenaCombatantClasses");

            migrationBuilder.DropColumn(
                name: "DefaultStageNameProfileId",
                table: "ArenaCombatantClasses");

            migrationBuilder.AddColumn<string>(
                name: "DefaultStageNameTemplate",
                table: "ArenaCombatantClasses",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }
    }
}
