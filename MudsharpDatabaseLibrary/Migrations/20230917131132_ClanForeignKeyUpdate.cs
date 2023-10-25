using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ClanForeignKeyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Appointments_Controlled",
                table: "ExternalClanControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Appointments_Controlling",
                table: "ExternalClanControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Clans_Liege",
                table: "ExternalClanControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Clans_Vassal",
                table: "ExternalClanControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Appointments_ExternalClanControls",
                table: "ExternalClanControls_Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Appointments_Controlled",
                table: "ExternalClanControls",
                column: "ControlledAppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Appointments_Controlling",
                table: "ExternalClanControls",
                column: "ControllingAppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Clans_Liege",
                table: "ExternalClanControls",
                column: "LiegeClanId",
                principalTable: "Clans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Clans_Vassal",
                table: "ExternalClanControls",
                column: "VassalClanId",
                principalTable: "Clans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Appointments_ExternalClanControls",
                table: "ExternalClanControls_Appointments",
                columns: new[] { "VassalClanId", "LiegeClanId", "ControlledAppointmentId" },
                principalTable: "ExternalClanControls",
                principalColumns: new[] { "VassalClanId", "LiegeClanId", "ControlledAppointmentId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Appointments_Controlled",
                table: "ExternalClanControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Appointments_Controlling",
                table: "ExternalClanControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Clans_Liege",
                table: "ExternalClanControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Clans_Vassal",
                table: "ExternalClanControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ECC_Appointments_ExternalClanControls",
                table: "ExternalClanControls_Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Appointments_Controlled",
                table: "ExternalClanControls",
                column: "ControlledAppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Appointments_Controlling",
                table: "ExternalClanControls",
                column: "ControllingAppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Clans_Liege",
                table: "ExternalClanControls",
                column: "LiegeClanId",
                principalTable: "Clans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Clans_Vassal",
                table: "ExternalClanControls",
                column: "VassalClanId",
                principalTable: "Clans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ECC_Appointments_ExternalClanControls",
                table: "ExternalClanControls_Appointments",
                columns: new[] { "VassalClanId", "LiegeClanId", "ControlledAppointmentId" },
                principalTable: "ExternalClanControls",
                principalColumns: new[] { "VassalClanId", "LiegeClanId", "ControlledAppointmentId" });
        }
    }
}
