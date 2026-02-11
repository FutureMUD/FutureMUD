using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AIStorytellers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIStorytellerReferenceDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FolderName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Keywords = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentContents = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RestrictedStorytellerIds = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIStorytellerReferenceDocuments", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AIStorytellers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Model = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SystemPrompt = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AttentionAgentPrompt = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SurveillanceStrategyDefinition = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReasoningEffort = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomToolCallsDefinition = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubscribeTo5mHeartbeat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SubscribeTo10mHeartbeat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SubscribeTo30mHeartbeat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SubscribeToHourHeartbeat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HeartbeatStatus5mProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    HeartbeatStatus10mProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    HeartbeatStatus30mProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    HeartbeatStatus1hProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsPaused = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SubscribeToRoomEvents = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIStorytellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIStorytellers_FutureProgs_HeartbeatStatus10mProgId",
                        column: x => x.HeartbeatStatus10mProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AIStorytellers_FutureProgs_HeartbeatStatus1hProgId",
                        column: x => x.HeartbeatStatus1hProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AIStorytellers_FutureProgs_HeartbeatStatus30mProgId",
                        column: x => x.HeartbeatStatus30mProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AIStorytellers_FutureProgs_HeartbeatStatus5mProgId",
                        column: x => x.HeartbeatStatus5mProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AIStorytellerCharacterMemories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AIStorytellerId = table.Column<long>(type: "bigint", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MemoryTitle = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MemoryText = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIStorytellerCharacterMemories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIStorytellerCharacterMemories_AIStorytellers_AIStorytellerId",
                        column: x => x.AIStorytellerId,
                        principalTable: "AIStorytellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AIStorytellerCharacterMemories_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AIStorytellerSituations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AIStorytellerId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SituationText = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsResolved = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIStorytellerSituations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIStorytellerSituations_AIStorytellers_AIStorytellerId",
                        column: x => x.AIStorytellerId,
                        principalTable: "AIStorytellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AIStorytellerCharacterMemories_AIStorytellerId",
                table: "AIStorytellerCharacterMemories",
                column: "AIStorytellerId");

            migrationBuilder.CreateIndex(
                name: "IX_AIStorytellerCharacterMemories_CharacterId",
                table: "AIStorytellerCharacterMemories",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_AIStorytellers_HeartbeatStatus10mProgId",
                table: "AIStorytellers",
                column: "HeartbeatStatus10mProgId");

            migrationBuilder.CreateIndex(
                name: "IX_AIStorytellers_HeartbeatStatus1hProgId",
                table: "AIStorytellers",
                column: "HeartbeatStatus1hProgId");

            migrationBuilder.CreateIndex(
                name: "IX_AIStorytellers_HeartbeatStatus30mProgId",
                table: "AIStorytellers",
                column: "HeartbeatStatus30mProgId");

            migrationBuilder.CreateIndex(
                name: "IX_AIStorytellers_HeartbeatStatus5mProgId",
                table: "AIStorytellers",
                column: "HeartbeatStatus5mProgId");

            migrationBuilder.CreateIndex(
                name: "IX_AIStorytellerSituations_AIStorytellerId",
                table: "AIStorytellerSituations",
                column: "AIStorytellerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIStorytellerCharacterMemories");

            migrationBuilder.DropTable(
                name: "AIStorytellerReferenceDocuments");

            migrationBuilder.DropTable(
                name: "AIStorytellerSituations");

            migrationBuilder.DropTable(
                name: "AIStorytellers");
        }
    }
}
