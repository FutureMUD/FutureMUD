using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class Elections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "ArchivedMembership",
                table: "ClanMemberships",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AlterColumn<long>(
                name: "CanNominateProgId",
                table: "Appointments",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WhyCantNominateProgId",
                table: "Appointments",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Elections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AppointmentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NominationStartDate = table.Column<string>(type: "varchar(100)", nullable: true),
                    VotingStartDate = table.Column<string>(type: "varchar(100)", nullable: true),
                    VotingEndDate = table.Column<string>(type: "varchar(100)", nullable: true),
                    ResultsInEffectDate = table.Column<string>(type: "varchar(100)", nullable: true),
                    IsFinalised = table.Column<ulong>(type: "bit(1)", nullable: false),
                    NumberOfAppointments = table.Column<int>(type: "int(11)", nullable: false),
                    IsByElection = table.Column<ulong>(type: "bit(1)", nullable: false),
                    ElectionStage = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Elections_Appointments",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectionsNominees",
                columns: table => new
                {
                    ElectionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NomineeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NomineeClanId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ElectionId, x.NomineeId });
                    table.ForeignKey(
                        name: "FK_ElectionsNominees_Elections",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionsNominees_ClanMemberships",
                        columns: x => new { x.NomineeClanId, x.NomineeId },
                        principalTable: "ClanMemberships",
                        principalColumns: new[] { "ClanId", "CharacterId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectionsVotes",
                columns: table => new
                {
                    ElectionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VoterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NomineeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VoterClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NomineeClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NumberOfVotes = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ElectionId, x.NomineeId, x.VoterId });
                    table.ForeignKey(
                        name: "FK_ElectionsVotes_Elections",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionsVotes_Nominees",
                        columns: x => new { x.NomineeClanId, x.NomineeId },
                        principalTable: "ClanMemberships",
                        principalColumns: new[] { "ClanId", "CharacterId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionsVotes_Voters",
                        columns: x => new { x.VoterClanId, x.VoterId },
                        principalTable: "ClanMemberships",
                        principalColumns: new[] { "ClanId", "CharacterId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_WhyCantNominateProg_idx",
                table: "Appointments",
                column: "WhyCantNominateProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Elections_Appointments_idx",
                table: "Elections",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "FK_ElectionsNominees_Elections_idx",
                table: "ElectionsNominees",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "FK_ElectionsNominees_ClanMemberships_idx",
                table: "ElectionsNominees",
                columns: new[] { "NomineeClanId", "NomineeId" });

            migrationBuilder.CreateIndex(
                name: "FK_ElectionsVotes_Elections_idx",
                table: "ElectionsVotes",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "FK_ElectionsVotes_Nominees_idx",
                table: "ElectionsVotes",
                columns: new[] { "NomineeClanId", "NomineeId" });

            migrationBuilder.CreateIndex(
                name: "FK_ElectionsVotes_Voters_idx",
                table: "ElectionsVotes",
                columns: new[] { "VoterClanId", "VoterId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_WhyCantNominateProg",
                table: "Appointments",
                column: "WhyCantNominateProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_WhyCantNominateProg",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "ElectionsNominees");

            migrationBuilder.DropTable(
                name: "ElectionsVotes");

            migrationBuilder.DropTable(
                name: "Elections");

            migrationBuilder.DropIndex(
                name: "FK_Appointments_WhyCantNominateProg_idx",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ArchivedMembership",
                table: "ClanMemberships");

            migrationBuilder.DropColumn(
                name: "WhyCantNominateProgId",
                table: "Appointments");

            migrationBuilder.AlterColumn<long>(
                name: "CanNominateProgId",
                table: "Appointments",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);
        }
    }
}
