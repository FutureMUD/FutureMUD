using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class ClanFKFixing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Clans",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Abbreviations_Appointments",
                table: "Appointments_Abbreviations");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Titles_Appointments",
                table: "Appointments_Titles");

            migrationBuilder.DropForeignKey(
                name: "FK_Paygrades_Clans",
                table: "Paygrades");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Clans",
                table: "Appointments",
                column: "ClanId",
                principalTable: "Clans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Abbreviations_Appointments",
                table: "Appointments_Abbreviations",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Titles_Appointments",
                table: "Appointments_Titles",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Paygrades_Clans",
                table: "Paygrades",
                column: "ClanId",
                principalTable: "Clans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Clans",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Abbreviations_Appointments",
                table: "Appointments_Abbreviations");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Titles_Appointments",
                table: "Appointments_Titles");

            migrationBuilder.DropForeignKey(
                name: "FK_Paygrades_Clans",
                table: "Paygrades");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Clans",
                table: "Appointments",
                column: "ClanId",
                principalTable: "Clans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Abbreviations_Appointments",
                table: "Appointments_Abbreviations",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Titles_Appointments",
                table: "Appointments_Titles",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Paygrades_Clans",
                table: "Paygrades",
                column: "ClanId",
                principalTable: "Clans",
                principalColumn: "Id");
        }
    }
}
