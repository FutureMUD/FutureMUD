using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class SurgicalProcedureCheckTraits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CheckTraitDefinitionId",
                table: "SurgicalProcedures",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurgicalProcedures_CheckTraitDefinitionId",
                table: "SurgicalProcedures",
                column: "CheckTraitDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurgicalProcedures_TraitDefinitions",
                table: "SurgicalProcedures",
                column: "CheckTraitDefinitionId",
                principalTable: "TraitDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurgicalProcedures_TraitDefinitions",
                table: "SurgicalProcedures");

            migrationBuilder.DropIndex(
                name: "IX_SurgicalProcedures_CheckTraitDefinitionId",
                table: "SurgicalProcedures");

            migrationBuilder.DropColumn(
                name: "CheckTraitDefinitionId",
                table: "SurgicalProcedures");
        }
    }
}
