using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class HeritageChargenCostBugFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cultures_ChargenResources_ChargenResources",
                table: "Cultures_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Cultures_ChargenResources_Races",
                table: "Cultures_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Ethnicities_ChargenResources_ChargenResources",
                table: "Ethnicities_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Ethnicities_ChargenResources_Ethnicities",
                table: "Ethnicities_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_ChargenResources_ChargenResources",
                table: "Races_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_ChargenResources_Races",
                table: "Races_ChargenResources");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "Races_ChargenResources");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "Ethnicities_ChargenResources");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "Cultures_ChargenResources");

            migrationBuilder.RenameIndex(
                name: "FK_Ethnicities_ChargenResources_ChargenResources",
                table: "Ethnicities_ChargenResources",
                newName: "IX_Ethnicities_ChargenResources_ChargenResourceId");

            migrationBuilder.RenameIndex(
                name: "FK_Cultures_ChargenResources_ChargenResources",
                table: "Cultures_ChargenResources",
                newName: "IX_Cultures_ChargenResources_ChargenResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "Races_ChargenResources",
                columns: new[] { "RaceId", "ChargenResourceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "Ethnicities_ChargenResources",
                columns: new[] { "EthnicityId", "ChargenResourceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "Cultures_ChargenResources",
                columns: new[] { "CultureId", "ChargenResourceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Cultures_ChargenResources_ChargenResources_ChargenResourceId",
                table: "Cultures_ChargenResources",
                column: "ChargenResourceId",
                principalTable: "ChargenResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cultures_ChargenResources_Cultures_CultureId",
                table: "Cultures_ChargenResources",
                column: "CultureId",
                principalTable: "Cultures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ethnicities_ChargenResources_ChargenResources_ChargenResourc~",
                table: "Ethnicities_ChargenResources",
                column: "ChargenResourceId",
                principalTable: "ChargenResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ethnicities_ChargenResources_Ethnicities_EthnicityId",
                table: "Ethnicities_ChargenResources",
                column: "EthnicityId",
                principalTable: "Ethnicities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_ChargenResources_ChargenResources_ChargenResourceId",
                table: "Races_ChargenResources",
                column: "ChargenResourceId",
                principalTable: "ChargenResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_ChargenResources_Races_RaceId",
                table: "Races_ChargenResources",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cultures_ChargenResources_ChargenResources_ChargenResourceId",
                table: "Cultures_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Cultures_ChargenResources_Cultures_CultureId",
                table: "Cultures_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Ethnicities_ChargenResources_ChargenResources_ChargenResourc~",
                table: "Ethnicities_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Ethnicities_ChargenResources_Ethnicities_EthnicityId",
                table: "Ethnicities_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_ChargenResources_ChargenResources_ChargenResourceId",
                table: "Races_ChargenResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_ChargenResources_Races_RaceId",
                table: "Races_ChargenResources");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "Races_ChargenResources");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "Ethnicities_ChargenResources");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "Cultures_ChargenResources");

            migrationBuilder.RenameIndex(
                name: "IX_Ethnicities_ChargenResources_ChargenResourceId",
                table: "Ethnicities_ChargenResources",
                newName: "FK_Ethnicities_ChargenResources_ChargenResources");

            migrationBuilder.RenameIndex(
                name: "IX_Cultures_ChargenResources_ChargenResourceId",
                table: "Cultures_ChargenResources",
                newName: "FK_Cultures_ChargenResources_ChargenResources");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "Races_ChargenResources",
                columns: new[] { "RaceId", "ChargenResourceId", "RequirementOnly" });

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "Ethnicities_ChargenResources",
                columns: new[] { "EthnicityId", "ChargenResourceId", "RequirementOnly" });

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "Cultures_ChargenResources",
                columns: new[] { "CultureId", "ChargenResourceId", "RequirementOnly" });

            migrationBuilder.AddForeignKey(
                name: "FK_Cultures_ChargenResources_ChargenResources",
                table: "Cultures_ChargenResources",
                column: "ChargenResourceId",
                principalTable: "ChargenResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cultures_ChargenResources_Races",
                table: "Cultures_ChargenResources",
                column: "CultureId",
                principalTable: "Cultures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ethnicities_ChargenResources_ChargenResources",
                table: "Ethnicities_ChargenResources",
                column: "ChargenResourceId",
                principalTable: "ChargenResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ethnicities_ChargenResources_Ethnicities",
                table: "Ethnicities_ChargenResources",
                column: "EthnicityId",
                principalTable: "Ethnicities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_ChargenResources_ChargenResources",
                table: "Races_ChargenResources",
                column: "ChargenResourceId",
                principalTable: "ChargenResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_ChargenResources_Races",
                table: "Races_ChargenResources",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
