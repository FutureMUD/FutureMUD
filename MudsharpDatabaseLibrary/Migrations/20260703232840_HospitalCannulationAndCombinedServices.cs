using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class HospitalCannulationAndCombinedServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AnesthesiaCannulationProcedureId",
                table: "HospitalServices",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServices_AnesthesiaCannulationProcedure_idx",
                table: "HospitalServices",
                column: "AnesthesiaCannulationProcedureId");

            migrationBuilder.AddForeignKey(
                name: "FK_HospitalServices_AnesthesiaCannulationProcedure",
                table: "HospitalServices",
                column: "AnesthesiaCannulationProcedureId",
                principalTable: "SurgicalProcedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HospitalServices_AnesthesiaCannulationProcedure",
                table: "HospitalServices");

            migrationBuilder.DropIndex(
                name: "FK_HospitalServices_AnesthesiaCannulationProcedure_idx",
                table: "HospitalServices");

            migrationBuilder.DropColumn(
                name: "AnesthesiaCannulationProcedureId",
                table: "HospitalServices");
        }
    }
}
