using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ClanElectionGovernance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "IsConfirmed",
                table: "Elections",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "CanDirectlyAppoint",
                table: "Appointments",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "CanResign",
                table: "Appointments",
                type: "bit(1)",
                nullable: false,
                defaultValue: 1ul);

            migrationBuilder.AddColumn<long>(
                name: "ElectionConfirmationAppointmentId",
                table: "Appointments",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ElectionConfirmationAppointmentId",
                table: "Appointments",
                column: "ElectionConfirmationAppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_ElectionConfirmation",
                table: "Appointments",
                column: "ElectionConfirmationAppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_ElectionConfirmation",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ElectionConfirmationAppointmentId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "CanDirectlyAppoint",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CanResign",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ElectionConfirmationAppointmentId",
                table: "Appointments");
        }
    }
}
