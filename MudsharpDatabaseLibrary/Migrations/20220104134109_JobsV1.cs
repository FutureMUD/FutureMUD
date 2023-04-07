using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class JobsV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobFindingLocations",
                columns: table => new
                {
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EconomicZoneId, x.CellId });
                    table.ForeignKey(
                        name: "FK_JobFindingLocations_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobFindingLocations_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JobListings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PosterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsReadyToBePosted = table.Column<ulong>(type: "bit(1)", nullable: false),
                    IsArchived = table.Column<ulong>(type: "bit(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PosterType = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    JobListingType = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    MoneyPaidIn = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    MaximumDuration = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EligibilityProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RankId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaygradeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AppointmentId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PersonalProjectId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PersonalProjectRevisionNumber = table.Column<int>(type: "int(11)", nullable: true),
                    RequiredProjectId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RequiredProjectLabourId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BankAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MaximumNumberOfSimultaneousEmployees = table.Column<int>(type: "int(11)", nullable: false),
                    FullTimeEquivalentRatio = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobListings_ActiveProjectLabours",
                        columns: x => new { x.RequiredProjectId, x.RequiredProjectLabourId },
                        principalTable: "ActiveProjectLabours",
                        principalColumns: new[] { "ActiveProjectId", "ProjectLabourRequirementsId" });
                    table.ForeignKey(
                        name: "FK_JobListings_ActiveProjects",
                        column: x => x.RequiredProjectId,
                        principalTable: "ActiveProjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobListings_Appointments",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobListings_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobListings_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobListings_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobListings_FutureProgs",
                        column: x => x.EligibilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobListings_Paygrades",
                        column: x => x.PaygradeId,
                        principalTable: "Paygrades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobListings_Projects",
                        columns: x => new { x.PersonalProjectId, x.PersonalProjectRevisionNumber },
                        principalTable: "Projects",
                        principalColumns: new[] { "Id", "RevisionNumber" });
                    table.ForeignKey(
                        name: "FK_JobListings_Ranks",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ActiveJobs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JobListingId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    JobCommenced = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    JobDueToEnd = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    JobEnded = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsJobComplete = table.Column<ulong>(type: "bit(1)", nullable: false),
                    AlreadyHadClanPosition = table.Column<ulong>(type: "bit(1)", nullable: false),
                    BackpayOwed = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RevenueEarned = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CurrentPerformance = table.Column<double>(type: "double", nullable: false),
                    ActiveProjectId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveJobs_ActiveProjects",
                        column: x => x.ActiveProjectId,
                        principalTable: "ActiveProjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveJobs_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveJobs_JobListings",
                        column: x => x.JobListingId,
                        principalTable: "JobListings",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveJobs_ActiveProjectId",
                table: "ActiveJobs",
                column: "ActiveProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveJobs_CharacterId",
                table: "ActiveJobs",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveJobs_JobListingId",
                table: "ActiveJobs",
                column: "JobListingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobFindingLocations_CellId",
                table: "JobFindingLocations",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_AppointmentId",
                table: "JobListings",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_BankAccountId",
                table: "JobListings",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_ClanId",
                table: "JobListings",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_EconomicZoneId",
                table: "JobListings",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_EligibilityProgId",
                table: "JobListings",
                column: "EligibilityProgId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_PaygradeId",
                table: "JobListings",
                column: "PaygradeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_PersonalProjectId_PersonalProjectRevisionNumber",
                table: "JobListings",
                columns: new[] { "PersonalProjectId", "PersonalProjectRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_RankId",
                table: "JobListings",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_RequiredProjectId_RequiredProjectLabourId",
                table: "JobListings",
                columns: new[] { "RequiredProjectId", "RequiredProjectLabourId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveJobs");

            migrationBuilder.DropTable(
                name: "JobFindingLocations");

            migrationBuilder.DropTable(
                name: "JobListings");
        }
    }
}
