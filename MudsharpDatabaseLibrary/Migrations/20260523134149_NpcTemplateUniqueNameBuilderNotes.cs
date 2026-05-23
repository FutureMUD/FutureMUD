using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class NpcTemplateUniqueNameBuilderNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuilderNotes",
                table: "NPCTemplates",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "UniqueName",
                table: "NPCTemplates",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateIndex(
                name: "IX_NPCTemplates_UniqueName",
                table: "NPCTemplates",
                column: "UniqueName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NPCTemplates_UniqueName",
                table: "NPCTemplates");

            migrationBuilder.DropColumn(
                name: "BuilderNotes",
                table: "NPCTemplates");

            migrationBuilder.DropColumn(
                name: "UniqueName",
                table: "NPCTemplates");
        }
    }
}
