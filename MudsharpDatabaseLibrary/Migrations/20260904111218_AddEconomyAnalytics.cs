using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddEconomyAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EconomicActivityRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RealDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FinancialPeriodId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MudCalendarId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MudYear = table.Column<int>(type: "int(11)", nullable: true),
                    MudMonth = table.Column<int>(type: "int(11)", nullable: true),
                    MudWeek = table.Column<int>(type: "int(11)", nullable: true),
                    MudDay = table.Column<int>(type: "int(11)", nullable: true),
                    MudDateTime = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActivityType = table.Column<int>(type: "int(11)", nullable: false),
                    VolumeClassification = table.Column<int>(type: "int(11)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    GlobalBaseValue = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    SourceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SourceType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceControlBucket = table.Column<int>(type: "int(11)", nullable: false),
                    DestinationId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DestinationType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinationControlBucket = table.Column<int>(type: "int(11)", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ReferenceType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceText = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EconomicActivityRecords_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EconomicActivityRecords_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EconomicActivityRecords_FinancialPeriods",
                        column: x => x.FinancialPeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EconomySnapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RealDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: true),
                    FinancialPeriodId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MudDateTime = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reason = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EconomySnapshots_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EconomySnapshots_FinancialPeriods",
                        column: x => x.FinancialPeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EconomySnapshotEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EconomySnapshotId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Metric = table.Column<int>(type: "int(11)", nullable: false),
                    ControlBucket = table.Column<int>(type: "int(11)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    GlobalBaseValue = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    EntityCount = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EconomySnapshotEntries_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EconomySnapshotEntries_Snapshots",
                        column: x => x.EconomySnapshotId,
                        principalTable: "EconomySnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_EconomicActivityRecords_Currencies_idx",
                table: "EconomicActivityRecords",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_EconomicActivityRecords_FinancialPeriodId",
                table: "EconomicActivityRecords",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EconomicActivityRecords_MudDate",
                table: "EconomicActivityRecords",
                columns: new[] { "MudCalendarId", "MudYear", "MudMonth", "MudDay" });

            migrationBuilder.CreateIndex(
                name: "IX_EconomicActivityRecords_RealDateTime",
                table: "EconomicActivityRecords",
                column: "RealDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_EconomicActivityRecords_Zone_FinancialPeriod",
                table: "EconomicActivityRecords",
                columns: new[] { "EconomicZoneId", "FinancialPeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_EconomicActivityRecords_Zone_RealDateTime",
                table: "EconomicActivityRecords",
                columns: new[] { "EconomicZoneId", "RealDateTime" });

            migrationBuilder.CreateIndex(
                name: "FK_EconomySnapshotEntries_Snapshots_idx",
                table: "EconomySnapshotEntries",
                column: "EconomySnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_EconomySnapshotEntries_Currency_Metric_Control",
                table: "EconomySnapshotEntries",
                columns: new[] { "CurrencyId", "Metric", "ControlBucket" });

            migrationBuilder.CreateIndex(
                name: "IX_EconomySnapshots_FinancialPeriodId",
                table: "EconomySnapshots",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EconomySnapshots_RealDateTime",
                table: "EconomySnapshots",
                column: "RealDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_EconomySnapshots_Zone_RealDateTime",
                table: "EconomySnapshots",
                columns: new[] { "EconomicZoneId", "RealDateTime" });

            migrationBuilder.CreateIndex(
                name: "UX_EconomySnapshots_Zone_Period_Reason",
                table: "EconomySnapshots",
                columns: new[] { "EconomicZoneId", "FinancialPeriodId", "Reason" },
                unique: true);

			migrationBuilder.InsertData(
				table: "StaticConfigurations",
				columns: new[] { "SettingName", "Definition" },
				values: new object[,]
				{
					{ "EconomyAnalyticsSnapshotsEnabled", "true" },
					{ "EconomyAnalyticsSnapshotIntervalMinutes", "1440" },
					{ "EconomyAnalyticsRolloverSnapshotsEnabled", "true" }
				});
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DeleteData(
				table: "StaticConfigurations",
				keyColumn: "SettingName",
				keyValues: new object[]
				{
					"EconomyAnalyticsSnapshotsEnabled",
					"EconomyAnalyticsSnapshotIntervalMinutes",
					"EconomyAnalyticsRolloverSnapshotsEnabled"
				});

            migrationBuilder.DropTable(
                name: "EconomicActivityRecords");

            migrationBuilder.DropTable(
                name: "EconomySnapshotEntries");

            migrationBuilder.DropTable(
                name: "EconomySnapshots");
        }
    }
}
