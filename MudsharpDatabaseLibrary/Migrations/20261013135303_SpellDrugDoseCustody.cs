using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class SpellDrugDoseCustody : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bodies_DrugDoses_BodyId",
                table: "Bodies_DrugDoses",
                column: "BodyId");

            // Keep the BodyId foreign key indexed and make the new identity column a key
            // in the same ALTER; MySQL does not allow a free-standing AUTO_INCREMENT column.
            migrationBuilder.Sql("ALTER TABLE `Bodies_DrugDoses` DROP PRIMARY KEY, " +
                "ADD COLUMN `Id` bigint(20) NOT NULL AUTO_INCREMENT, ADD PRIMARY KEY (`Id`);");

            migrationBuilder.AddColumn<string>(
                name: "SpellOriginFamily",
                table: "Bodies_DrugDoses",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "SpellOriginId",
                table: "Bodies_DrugDoses",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "SpellOriginFormula",
                table: "Bodies_DrugDoses",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // A rollback must not silently collapse two distinct spell-owned doses.
            migrationBuilder.Sql("ALTER TABLE `Bodies_DrugDoses` DROP PRIMARY KEY, " +
                "DROP COLUMN `Id`, ADD PRIMARY KEY (`BodyId`, `DrugId`, `Active`);");

            migrationBuilder.DropIndex(
                name: "IX_Bodies_DrugDoses_BodyId",
                table: "Bodies_DrugDoses");

            migrationBuilder.DropColumn(
                name: "SpellOriginFamily",
                table: "Bodies_DrugDoses");

            migrationBuilder.DropColumn(
                name: "SpellOriginId",
                table: "Bodies_DrugDoses");

            migrationBuilder.DropColumn(
                name: "SpellOriginFormula",
                table: "Bodies_DrugDoses");

        }
    }
}
