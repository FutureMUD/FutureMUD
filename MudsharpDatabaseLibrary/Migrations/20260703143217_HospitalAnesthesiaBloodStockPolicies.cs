using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class HospitalAnesthesiaBloodStockPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AnesthesiaDrugId",
                table: "HospitalServices",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AnesthesiaIntensity",
                table: "HospitalServices",
                type: "double",
                nullable: false,
                defaultValue: 1.25);

            migrationBuilder.AddColumn<long>(
                name: "ImplantInterfaceProcedureId",
                table: "HospitalServices",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ImplantPowerProcedureId",
                table: "HospitalServices",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HospitalBloodStockPolicies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HospitalId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BloodtypeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TargetLitres = table.Column<double>(type: "double", nullable: false),
                    PricePerLitre = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HospitalBloodStockPolicies_Bloodtypes",
                        column: x => x.BloodtypeId,
                        principalTable: "Bloodtypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HospitalBloodStockPolicies_Hospitals",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServices_Drugs_Anesthesia_idx",
                table: "HospitalServices",
                column: "AnesthesiaDrugId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServices_ImplantInterfaceProcedure_idx",
                table: "HospitalServices",
                column: "ImplantInterfaceProcedureId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServices_ImplantPowerProcedure_idx",
                table: "HospitalServices",
                column: "ImplantPowerProcedureId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalBloodStockPolicies_Bloodtypes_idx",
                table: "HospitalBloodStockPolicies",
                column: "BloodtypeId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalBloodStockPolicies_Hospitals_idx",
                table: "HospitalBloodStockPolicies",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalBloodStockPolicies_Hospital_Bloodtype",
                table: "HospitalBloodStockPolicies",
                columns: new[] { "HospitalId", "BloodtypeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HospitalServices_Drugs_Anesthesia",
                table: "HospitalServices",
                column: "AnesthesiaDrugId",
                principalTable: "Drugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_HospitalServices_ImplantInterfaceProcedure",
                table: "HospitalServices",
                column: "ImplantInterfaceProcedureId",
                principalTable: "SurgicalProcedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_HospitalServices_ImplantPowerProcedure",
                table: "HospitalServices",
                column: "ImplantPowerProcedureId",
                principalTable: "SurgicalProcedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HospitalServices_Drugs_Anesthesia",
                table: "HospitalServices");

            migrationBuilder.DropForeignKey(
                name: "FK_HospitalServices_ImplantInterfaceProcedure",
                table: "HospitalServices");

            migrationBuilder.DropForeignKey(
                name: "FK_HospitalServices_ImplantPowerProcedure",
                table: "HospitalServices");

            migrationBuilder.DropTable(
                name: "HospitalBloodStockPolicies");

            migrationBuilder.DropIndex(
                name: "FK_HospitalServices_Drugs_Anesthesia_idx",
                table: "HospitalServices");

            migrationBuilder.DropIndex(
                name: "FK_HospitalServices_ImplantInterfaceProcedure_idx",
                table: "HospitalServices");

            migrationBuilder.DropIndex(
                name: "FK_HospitalServices_ImplantPowerProcedure_idx",
                table: "HospitalServices");

            migrationBuilder.DropColumn(
                name: "AnesthesiaDrugId",
                table: "HospitalServices");

            migrationBuilder.DropColumn(
                name: "AnesthesiaIntensity",
                table: "HospitalServices");

            migrationBuilder.DropColumn(
                name: "ImplantInterfaceProcedureId",
                table: "HospitalServices");

            migrationBuilder.DropColumn(
                name: "ImplantPowerProcedureId",
                table: "HospitalServices");
        }
    }
}
