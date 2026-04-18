using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddComputerMailService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComputerMailDomains",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DomainName = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    HostItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Enabled = table.Column<ulong>(type: "bit(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComputerMailDomains_GameItems",
                        column: x => x.HostItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ComputerMailMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SenderAddress = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RecipientAddress = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Subject = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Body = table.Column<string>(type: "longtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    SentAtUtc = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ComputerMailAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ComputerMailDomainId = table.Column<long>(type: "bigint(20)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PasswordHash = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PasswordSalt = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsEnabled = table.Column<ulong>(type: "bit(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComputerMailAccounts_ComputerMailDomains",
                        column: x => x.ComputerMailDomainId,
                        principalTable: "ComputerMailDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ComputerMailMailboxEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ComputerMailAccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ComputerMailMessageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsSentFolder = table.Column<ulong>(type: "bit(1)", nullable: false),
                    IsRead = table.Column<ulong>(type: "bit(1)", nullable: false),
                    IsDeleted = table.Column<ulong>(type: "bit(1)", nullable: false),
                    DeliveredAtUtc = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComputerMailMailboxEntries_ComputerMailAccounts",
                        column: x => x.ComputerMailAccountId,
                        principalTable: "ComputerMailAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComputerMailMailboxEntries_ComputerMailMessages",
                        column: x => x.ComputerMailMessageId,
                        principalTable: "ComputerMailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_ComputerMailAccounts_ComputerMailDomains_idx",
                table: "ComputerMailAccounts",
                column: "ComputerMailDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_ComputerMailAccounts_Domain_UserName",
                table: "ComputerMailAccounts",
                columns: new[] { "ComputerMailDomainId", "UserName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_ComputerMailDomains_GameItems_idx",
                table: "ComputerMailDomains",
                column: "HostItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ComputerMailDomains_DomainName",
                table: "ComputerMailDomains",
                column: "DomainName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_ComputerMailMailboxEntries_ComputerMailAccounts_idx",
                table: "ComputerMailMailboxEntries",
                column: "ComputerMailAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_ComputerMailMailboxEntries_ComputerMailMessages_idx",
                table: "ComputerMailMailboxEntries",
                column: "ComputerMailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ComputerMailMailboxEntries_Account_Folder_Delivered",
                table: "ComputerMailMailboxEntries",
                columns: new[] { "ComputerMailAccountId", "IsDeleted", "IsSentFolder", "DeliveredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ComputerMailMessages_SentAtUtc",
                table: "ComputerMailMessages",
                column: "SentAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComputerMailMailboxEntries");

            migrationBuilder.DropTable(
                name: "ComputerMailAccounts");

            migrationBuilder.DropTable(
                name: "ComputerMailMessages");

            migrationBuilder.DropTable(
                name: "ComputerMailDomains");
        }
    }
}
