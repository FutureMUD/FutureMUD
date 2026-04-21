using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ProjectQueueAndCancellationContinuity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentProjectProjectHours",
                table: "Characters",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "ProjectLabourQueues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ActiveProjectId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProjectLabourRequirementId = table.Column<long>(type: "bigint(20)", nullable: false),
                    QueueOrder = table.Column<int>(type: "int(11)", nullable: false),
                    QueuedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLabourQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLabourQueues_ActiveProjects",
                        column: x => x.ActiveProjectId,
                        principalTable: "ActiveProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectLabourQueues_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectLabourQueues_ProjectLabourRequirements",
                        column: x => x.ProjectLabourRequirementId,
                        principalTable: "ProjectLabourRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectLabourQueues_ActiveProjects_idx",
                table: "ProjectLabourQueues",
                column: "ActiveProjectId");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectLabourQueues_Characters_idx",
                table: "ProjectLabourQueues",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectLabourQueues_ProjectLabourRequirements_idx",
                table: "ProjectLabourQueues",
                column: "ProjectLabourRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLabourQueues_Character_QueueOrder",
                table: "ProjectLabourQueues",
                columns: new[] { "CharacterId", "QueueOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "CurrentProjectProjectHours",
                table: "Characters");
        }
    }
}
