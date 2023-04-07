using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class MaterialsRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GasFormId",
                table: "Liquids",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DrugGramsPerUnitVolume",
                table: "Gases",
                type: "double",
                nullable: false,
                defaultValueSql: "'0.0'");

            migrationBuilder.AddColumn<long>(
                name: "DrugId",
                table: "Gases",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Liquids_GasFormId",
                table: "Liquids",
                column: "GasFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Gases_DrugId",
                table: "Gases",
                column: "DrugId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gases_Drugs",
                table: "Gases",
                column: "DrugId",
                principalTable: "Drugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Liquids_Gases",
                table: "Liquids",
                column: "GasFormId",
                principalTable: "Gases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gases_Drugs",
                table: "Gases");

            migrationBuilder.DropForeignKey(
                name: "FK_Liquids_Gases",
                table: "Liquids");

            migrationBuilder.DropIndex(
                name: "IX_Liquids_GasFormId",
                table: "Liquids");

            migrationBuilder.DropIndex(
                name: "IX_Gases_DrugId",
                table: "Gases");

            migrationBuilder.DropColumn(
                name: "GasFormId",
                table: "Liquids");

            migrationBuilder.DropColumn(
                name: "DrugGramsPerUnitVolume",
                table: "Gases");

            migrationBuilder.DropColumn(
                name: "DrugId",
                table: "Gases");
        }
    }
}
