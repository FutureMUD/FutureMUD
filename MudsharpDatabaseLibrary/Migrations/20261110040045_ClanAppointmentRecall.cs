using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ClanAppointmentRecall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "IsRecall",
                table: "Elections",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<double>(
                name: "RecallThreshold",
                table: "Elections",
                type: "double",
                nullable: false,
                defaultValue: 0.5);

            migrationBuilder.AddColumn<ulong>(
                name: "CanBeRecalled",
                table: "Appointments",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<long>(
                name: "CanInitiateRecallProgId",
                table: "Appointments",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "CanParentDismiss",
                table: "Appointments",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<double>(
                name: "RecallThreshold",
                table: "Appointments",
                type: "double",
                nullable: false,
                defaultValue: 0.5);

            migrationBuilder.AddColumn<long>(
                name: "RecallVotesProgId",
                table: "Appointments",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CanInitiateRecallProgId",
                table: "Appointments",
                column: "CanInitiateRecallProgId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_RecallVotesProgId",
                table: "Appointments",
                column: "RecallVotesProgId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_CanInitiateRecallProg",
                table: "Appointments",
                column: "CanInitiateRecallProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_RecallVotesProg",
                table: "Appointments",
                column: "RecallVotesProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_CanInitiateRecallProg",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_RecallVotesProg",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CanInitiateRecallProgId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_RecallVotesProgId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsRecall",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "RecallThreshold",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "CanBeRecalled",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CanInitiateRecallProgId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CanParentDismiss",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "RecallThreshold",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "RecallVotesProgId",
                table: "Appointments");
        }
    }
}
