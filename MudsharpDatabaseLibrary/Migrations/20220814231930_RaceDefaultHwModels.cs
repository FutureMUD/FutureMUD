using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class RaceDefaultHwModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DefaultHeightWeightModelFemaleId",
                table: "Races",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultHeightWeightModelMaleId",
                table: "Races",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultHeightWeightModelNeuterId",
                table: "Races",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultHeightWeightModelNonBinaryId",
                table: "Races",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Races_DefaultHeightWeightModelFemaleId",
                table: "Races",
                column: "DefaultHeightWeightModelFemaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_DefaultHeightWeightModelMaleId",
                table: "Races",
                column: "DefaultHeightWeightModelMaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_DefaultHeightWeightModelNeuterId",
                table: "Races",
                column: "DefaultHeightWeightModelNeuterId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_DefaultHeightWeightModelNonBinaryId",
                table: "Races",
                column: "DefaultHeightWeightModelNonBinaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Races_HeightWeightModelsFemale",
                table: "Races",
                column: "DefaultHeightWeightModelFemaleId",
                principalTable: "HeightWeightModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Races_HeightWeightModelsMale",
                table: "Races",
                column: "DefaultHeightWeightModelMaleId",
                principalTable: "HeightWeightModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Races_HeightWeightModelsNeuter",
                table: "Races",
                column: "DefaultHeightWeightModelNeuterId",
                principalTable: "HeightWeightModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Races_HeightWeightModelsNonBinary",
                table: "Races",
                column: "DefaultHeightWeightModelNonBinaryId",
                principalTable: "HeightWeightModels",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Races_HeightWeightModelsFemale",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_HeightWeightModelsMale",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_HeightWeightModelsNeuter",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_HeightWeightModelsNonBinary",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_Races_DefaultHeightWeightModelFemaleId",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_Races_DefaultHeightWeightModelMaleId",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_Races_DefaultHeightWeightModelNeuterId",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_Races_DefaultHeightWeightModelNonBinaryId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "DefaultHeightWeightModelFemaleId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "DefaultHeightWeightModelMaleId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "DefaultHeightWeightModelNeuterId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "DefaultHeightWeightModelNonBinaryId",
                table: "Races");
        }
    }
}
