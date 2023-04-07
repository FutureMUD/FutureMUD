using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class LineOfCreditAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LineOfCreditAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountName = table.Column<string>(nullable: true),
                    ShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsSuspended = table.Column<ulong>(type: "bit(1)", nullable: false),
                    AccountLimit = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    OutstandingBalance = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    AccountOwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccountOwnerName = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineOfCreditAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineOfCreditAccounts_Characters",
                        column: x => x.AccountOwnerId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineOfCreditAccounts_Shops",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineOfCreditAccountUsers",
                columns: table => new
                {
                    LineOfCreditAccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccountUserId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccountUserName = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    SpendingLimit = table.Column<decimal>(type: "decimal(10,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.LineOfCreditAccountId, x.AccountUserId });
                    table.ForeignKey(
                        name: "FK_LineOfCreditAccountUsers_Characters",
                        column: x => x.AccountUserId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineOfCreditAccountUsers_LineOfCreditAccounts",
                        column: x => x.LineOfCreditAccountId,
                        principalTable: "LineOfCreditAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "FK_LineOfCreditAccounts_Characters_idx",
                table: "LineOfCreditAccounts",
                column: "AccountOwnerId");

            migrationBuilder.CreateIndex(
                name: "FK_LineOfCreditAccounts_Shops_idx",
                table: "LineOfCreditAccounts",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "FK_LineOfCreditAccountUsers_Characters_idx",
                table: "LineOfCreditAccountUsers",
                column: "AccountUserId");

            migrationBuilder.CreateIndex(
                name: "FK_LineOfCreditAccountUsers_LineOfCreditAccounts_idx",
                table: "LineOfCreditAccountUsers",
                column: "LineOfCreditAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineOfCreditAccountUsers");

            migrationBuilder.DropTable(
                name: "LineOfCreditAccounts");
        }
    }
}
