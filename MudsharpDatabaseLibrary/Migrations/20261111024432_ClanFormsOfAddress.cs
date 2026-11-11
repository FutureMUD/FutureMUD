using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ClanFormsOfAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowFormsOfAddressInLook",
                table: "Clans",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "AddressPriority",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppointmentFormsOfAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AppointmentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Relations = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TemplateKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentFormsOfAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentFormsOfAddress_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentFormsOfAddress_FutureProgs_FutureProgId",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RankFormsOfAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Relations = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TemplateKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankFormsOfAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RankFormsOfAddress_FutureProgs_FutureProgId",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RankFormsOfAddress_Ranks_RankId",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentFormsOfAddress_AppointmentId_Order",
                table: "AppointmentFormsOfAddress",
                columns: new[] { "AppointmentId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentFormsOfAddress_FutureProgId",
                table: "AppointmentFormsOfAddress",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "IX_RankFormsOfAddress_FutureProgId",
                table: "RankFormsOfAddress",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "IX_RankFormsOfAddress_RankId_Order",
                table: "RankFormsOfAddress",
                columns: new[] { "RankId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentFormsOfAddress");

            migrationBuilder.DropTable(
                name: "RankFormsOfAddress");

            migrationBuilder.DropColumn(
                name: "ShowFormsOfAddressInLook",
                table: "Clans");

            migrationBuilder.DropColumn(
                name: "AddressPriority",
                table: "Appointments");
        }
    }
}
