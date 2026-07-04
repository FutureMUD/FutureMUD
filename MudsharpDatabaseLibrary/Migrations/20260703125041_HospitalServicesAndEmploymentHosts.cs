using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class HospitalServicesAndEmploymentHosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hospitals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BankAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsTrading = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    DefaultMaximumDebt = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hospitals_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Hospitals_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HospitalLocations",
                columns: table => new
                {
                    HospitalId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Role = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.HospitalId, x.CellId, x.Role });
                    table.ForeignKey(
                        name: "FK_HospitalLocations_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HospitalLocations_Hospitals",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HospitalPatientDebtAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HospitalId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PatientId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PatientName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Balance = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    MaximumDebt = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    IsSuspended = table.Column<ulong>(type: "bit(1)", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HospitalPatientDebtAccounts_Characters",
                        column: x => x.PatientId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HospitalPatientDebtAccounts_Hospitals",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HospitalServices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HospitalId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Keywords = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ServiceType = table.Column<int>(type: "int(11)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    IsActive = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    AllowDebt = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    PreferOperatingTheatre = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 0ul),
                    SortOrder = table.Column<int>(type: "int(11)", nullable: false),
                    SurgicalProcedureId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ImplantItemPrototypeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ImplantItemPrototypeRevisionNumber = table.Column<int>(type: "int(11)", nullable: true),
                    ProcedureParameters = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RequiredEquipmentJson = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    BloodVolumeLitres = table.Column<double>(type: "double", nullable: false, defaultValue: 0.5),
                    RequiresRecovery = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 0ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HospitalServices_GameItemProtos",
                        columns: x => new { x.ImplantItemPrototypeId, x.ImplantItemPrototypeRevisionNumber },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HospitalServices_Hospitals",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HospitalServices_SurgicalProcedures",
                        column: x => x.SurgicalProcedureId,
                        principalTable: "SurgicalProcedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HospitalServiceRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HospitalId = table.Column<long>(type: "bigint(20)", nullable: false),
                    HospitalServiceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequesterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequesterName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PatientId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PatientName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int(11)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    DebtCharged = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    EmploymentTaskId = table.Column<string>(type: "varchar(36)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AssignedEmployeeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OperatingTheatreCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    UsedInPlaceFallback = table.Column<ulong>(type: "bit(1)", nullable: false),
                    SupplyPrepared = table.Column<ulong>(type: "bit(1)", nullable: false),
                    PreparedByEmployeeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PreparedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RecoveryRoomCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ReturnCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OperationalNotes = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HospitalServiceRequests_Cells_Recovery",
                        column: x => x.RecoveryRoomCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HospitalServiceRequests_Cells_Return",
                        column: x => x.ReturnCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HospitalServiceRequests_Cells_Theatre",
                        column: x => x.OperatingTheatreCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HospitalServiceRequests_Characters_Employee",
                        column: x => x.AssignedEmployeeId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HospitalServiceRequests_Characters_Patient",
                        column: x => x.PatientId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HospitalServiceRequests_Characters_PreparedBy",
                        column: x => x.PreparedByEmployeeId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HospitalServiceRequests_Characters_Requester",
                        column: x => x.RequesterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HospitalServiceRequests_HospitalServices",
                        column: x => x.HospitalServiceId,
                        principalTable: "HospitalServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HospitalServiceRequests_Hospitals",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalLocations_Cells_idx",
                table: "HospitalLocations",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalLocations_Hospital_Role",
                table: "HospitalLocations",
                columns: new[] { "HospitalId", "Role" });

            migrationBuilder.CreateIndex(
                name: "FK_HospitalPatientDebtAccounts_Characters_idx",
                table: "HospitalPatientDebtAccounts",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalPatientDebtAccounts_Hospitals_idx",
                table: "HospitalPatientDebtAccounts",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalPatientDebtAccounts_Hospital_Patient",
                table: "HospitalPatientDebtAccounts",
                columns: new[] { "HospitalId", "PatientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_Hospitals_BankAccounts_idx",
                table: "Hospitals",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_Hospitals_EconomicZones_idx",
                table: "Hospitals",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServiceRequests_Cells_Recovery_idx",
                table: "HospitalServiceRequests",
                column: "RecoveryRoomCellId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServiceRequests_Cells_Return_idx",
                table: "HospitalServiceRequests",
                column: "ReturnCellId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServiceRequests_Cells_Theatre_idx",
                table: "HospitalServiceRequests",
                column: "OperatingTheatreCellId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServiceRequests_Characters_Employee_idx",
                table: "HospitalServiceRequests",
                column: "AssignedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServiceRequests_Characters_Patient_idx",
                table: "HospitalServiceRequests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServiceRequests_Characters_PreparedBy_idx",
                table: "HospitalServiceRequests",
                column: "PreparedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServiceRequests_Characters_Requester_idx",
                table: "HospitalServiceRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServiceRequests_Hospitals_idx",
                table: "HospitalServiceRequests",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServiceRequests_HospitalServices_idx",
                table: "HospitalServiceRequests",
                column: "HospitalServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalServiceRequests_EmploymentTaskId",
                table: "HospitalServiceRequests",
                column: "EmploymentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalServiceRequests_Hospital_Status",
                table: "HospitalServiceRequests",
                columns: new[] { "HospitalId", "Status" });

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServices_GameItemProtos_idx",
                table: "HospitalServices",
                columns: new[] { "ImplantItemPrototypeId", "ImplantItemPrototypeRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServices_Hospitals_idx",
                table: "HospitalServices",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "FK_HospitalServices_SurgicalProcedures_idx",
                table: "HospitalServices",
                column: "SurgicalProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalServices_Hospital_Name",
                table: "HospitalServices",
                columns: new[] { "HospitalId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HospitalLocations");

            migrationBuilder.DropTable(
                name: "HospitalPatientDebtAccounts");

            migrationBuilder.DropTable(
                name: "HospitalServiceRequests");

            migrationBuilder.DropTable(
                name: "HospitalServices");

            migrationBuilder.DropTable(
                name: "Hospitals");

        }
    }
}
