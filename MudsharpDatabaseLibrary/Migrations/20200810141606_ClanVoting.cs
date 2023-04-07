using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class ClanVoting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThirdPartyIItemType",
                table: "Crimes",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPartyId",
                table: "Crimes",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CanNominateProgId",
                table: "Appointments",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ElectionLeadTimeMinutes",
                table: "Appointments",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ElectionTermMinutes",
                table: "Appointments",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "IsAppointedByElection",
                table: "Appointments",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "IsSecretBallot",
                table: "Appointments",
                type: "bit(1)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumConsecutiveTerms",
                table: "Appointments",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumTotalTerms",
                table: "Appointments",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NominationPeriodMinutes",
                table: "Appointments",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NumberOfVotesProgId",
                table: "Appointments",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VotingPeriodMinutes",
                table: "Appointments",
                type: "double",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_CanNominateProg_idx",
                table: "Appointments",
                column: "CanNominateProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_NumberOfVotesProg_idx",
                table: "Appointments",
                column: "NumberOfVotesProgId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_CanNominateProg",
                table: "Appointments",
                column: "CanNominateProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_NumberOfVotesProg",
                table: "Appointments",
                column: "NumberOfVotesProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_CanNominateProg",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_NumberOfVotesProg",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "FK_Appointments_CanNominateProg_idx",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "FK_Appointments_NumberOfVotesProg_idx",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ThirdPartyIItemType",
                table: "Crimes");

            migrationBuilder.DropColumn(
                name: "ThirdPartyId",
                table: "Crimes");

            migrationBuilder.DropColumn(
                name: "CanNominateProgId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ElectionLeadTimeMinutes",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ElectionTermMinutes",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsAppointedByElection",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsSecretBallot",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "MaximumConsecutiveTerms",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "MaximumTotalTerms",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "NominationPeriodMinutes",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "NumberOfVotesProgId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "VotingPeriodMinutes",
                table: "Appointments");
        }
    }
}
