using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddLendingContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LendingProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LenderId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LenderType = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LendingProducts_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LendingContracts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LenderId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LenderType = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    BorrowerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityKey = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EstateId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Principal = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Surplus = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LendingContracts_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LendingContracts_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LendingContracts_Estates",
                        column: x => x.EstateId,
                        principalTable: "Estates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LendingContracts_LendingProducts",
                        column: x => x.ProductId,
                        principalTable: "LendingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LendingPostings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OperationKey = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Kind = table.Column<string>(type: "varchar(40)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    MudDate = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Detail = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RealDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Principal = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LendingPostings_LendingContracts",
                        column: x => x.ContractId,
                        principalTable: "LendingContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LendingContracts_BorrowerId_EconomicZoneId_Status",
                table: "LendingContracts",
                columns: new[] { "BorrowerId", "EconomicZoneId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LendingContracts_CurrencyId",
                table: "LendingContracts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_LendingContracts_EconomicZoneId",
                table: "LendingContracts",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_LendingContracts_EstateId",
                table: "LendingContracts",
                column: "EstateId");

            migrationBuilder.CreateIndex(
                name: "IX_LendingContracts_LenderId_LenderType_Status",
                table: "LendingContracts",
                columns: new[] { "LenderId", "LenderType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LendingContracts_ProductId",
                table: "LendingContracts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LendingContracts_SecurityKey",
                table: "LendingContracts",
                column: "SecurityKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LendingPostings_ActorId",
                table: "LendingPostings",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_LendingPostings_ContractId_OperationKey",
                table: "LendingPostings",
                columns: new[] { "ContractId", "OperationKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LendingPostings_ContractId_RealDate",
                table: "LendingPostings",
                columns: new[] { "ContractId", "RealDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LendingProducts_EconomicZoneId",
                table: "LendingProducts",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_LendingProducts_LenderId_LenderType_Name",
                table: "LendingProducts",
                columns: new[] { "LenderId", "LenderType", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LendingPostings");

            migrationBuilder.DropTable(
                name: "LendingContracts");

            migrationBuilder.DropTable(
                name: "LendingProducts");
        }
    }
}
