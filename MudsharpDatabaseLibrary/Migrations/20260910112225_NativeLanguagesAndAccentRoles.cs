using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class NativeLanguagesAndAccentRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Accents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Preserve the old learner choice before its relationship is removed.
            migrationBuilder.Sql("UPDATE Accents a INNER JOIN Languages l ON l.DefaultLearnerAccentId = a.Id SET a.Role = 2;");

            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Accents",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "FK_Languages_Accents_idx",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "DefaultLearnerAccentId",
                table: "Languages");

            migrationBuilder.AddColumn<long>(
                name: "NativeLanguageId",
                table: "Ethnicities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NativeLanguageId",
                table: "Cultures",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AcquisitionAccentId",
                table: "Characters_Languages",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NativeLanguageId",
                table: "Characters",
                type: "bigint(20)",
                nullable: true);


            migrationBuilder.CreateTable(
                name: "AccentsAssociatedLanguages",
                columns: table => new
                {
                    AccentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccentsAssociatedLanguages", x => new { x.AccentId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_AccentsAssociatedLanguages_Accents_AccentId",
                        column: x => x.AccentId,
                        principalTable: "Accents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccentsAssociatedLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Ethnicities_NativeLanguageId",
                table: "Ethnicities",
                column: "NativeLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Cultures_NativeLanguageId",
                table: "Cultures",
                column: "NativeLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Languages_AcquisitionAccentId",
                table: "Characters_Languages",
                column: "AcquisitionAccentId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_NativeLanguageId",
                table: "Characters",
                column: "NativeLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_AccentsAssociatedLanguages_LanguageId",
                table: "AccentsAssociatedLanguages",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Languages_NativeLanguageId",
                table: "Characters",
                column: "NativeLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Languages_Accents_AcquisitionAccentId",
                table: "Characters_Languages",
                column: "AcquisitionAccentId",
                principalTable: "Accents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Cultures_Languages_NativeLanguageId",
                table: "Cultures",
                column: "NativeLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Ethnicities_Languages_NativeLanguageId",
                table: "Ethnicities",
                column: "NativeLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Languages_NativeLanguageId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Languages_Accents_AcquisitionAccentId",
                table: "Characters_Languages");

            migrationBuilder.DropForeignKey(
                name: "FK_Cultures_Languages_NativeLanguageId",
                table: "Cultures");

            migrationBuilder.DropForeignKey(
                name: "FK_Ethnicities_Languages_NativeLanguageId",
                table: "Ethnicities");

            migrationBuilder.DropTable(
                name: "AccentsAssociatedLanguages");

            migrationBuilder.DropIndex(
                name: "IX_Ethnicities_NativeLanguageId",
                table: "Ethnicities");

            migrationBuilder.DropIndex(
                name: "IX_Cultures_NativeLanguageId",
                table: "Cultures");

            migrationBuilder.DropIndex(
                name: "IX_Characters_Languages_AcquisitionAccentId",
                table: "Characters_Languages");

            migrationBuilder.DropIndex(
                name: "IX_Characters_NativeLanguageId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "NativeLanguageId",
                table: "Ethnicities");

            migrationBuilder.DropColumn(
                name: "NativeLanguageId",
                table: "Cultures");

            migrationBuilder.DropColumn(
                name: "AcquisitionAccentId",
                table: "Characters_Languages");

            migrationBuilder.DropColumn(
                name: "NativeLanguageId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Accents");

            migrationBuilder.AddColumn<long>(
                name: "DefaultLearnerAccentId",
                table: "Languages",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_Languages_Accents_idx",
                table: "Languages",
                column: "DefaultLearnerAccentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Accents",
                table: "Languages",
                column: "DefaultLearnerAccentId",
                principalTable: "Accents",
                principalColumn: "Id");
        }
    }
}
