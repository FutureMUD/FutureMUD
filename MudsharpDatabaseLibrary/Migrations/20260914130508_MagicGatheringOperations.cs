using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MagicGatheringOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MagicGatheringOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MagicCapabilityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MethodKey = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MethodVersion = table.Column<int>(type: "int", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SourceProfileId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SourceProfileRevision = table.Column<long>(type: "bigint(20)", nullable: true),
                    SourceResourceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DestinationResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Kind = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestedAmount = table.Column<double>(type: "double", nullable: false),
                    SourceDebit = table.Column<double>(type: "double", nullable: false),
                    StaminaCost = table.Column<double>(type: "double", nullable: false),
                    DamageCost = table.Column<double>(type: "double", nullable: false),
                    PainCost = table.Column<double>(type: "double", nullable: false),
                    StunCost = table.Column<double>(type: "double", nullable: false),
                    SourceDebited = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BodilyCostApplied = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DestinationCredited = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccountingPersisted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotificationCompleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Diagnostic = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MagicGatheringOperations_Capability_Method",
                table: "MagicGatheringOperations",
                columns: new[] { "MagicCapabilityId", "MethodKey" });

            migrationBuilder.CreateIndex(
                name: "IX_MagicGatheringOperations_Cell_Source_Status",
                table: "MagicGatheringOperations",
                columns: new[] { "CellId", "SourceResourceId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_MagicGatheringOperations_OwnerId_Status",
                table: "MagicGatheringOperations",
                columns: new[] { "OwnerId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MagicGatheringOperations");
        }
    }
}
