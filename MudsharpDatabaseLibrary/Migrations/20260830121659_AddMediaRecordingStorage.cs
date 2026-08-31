using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaRecordingStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaRecordings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchemaVersion = table.Column<int>(type: "int(11)", nullable: false, defaultValue: 1),
                    Capabilities = table.Column<int>(type: "int(11)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false),
                    FinalisedAtUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    DurationMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false),
                    LogicalSizeInBytes = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MediaSceneSnapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContentHash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: false, collation: "ascii_general_ci")
                        .Annotation("MySql:CharSet", "ascii"),
                    UncompressedSizeBytes = table.Column<int>(type: "int(11)", nullable: false),
                    Payload = table.Column<byte[]>(type: "longblob", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MediaRecordingChunks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MediaRecordingId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Sequence = table.Column<int>(type: "int(11)", nullable: false),
                    OffsetMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false),
                    DurationMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false),
                    UncompressedSizeBytes = table.Column<int>(type: "int(11)", nullable: false),
                    Payload = table.Column<byte[]>(type: "longblob", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaRecordingChunks_MediaRecordings",
                        column: x => x.MediaRecordingId,
                        principalTable: "MediaRecordings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MediaRecordingReferences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameItemComponentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MediaRecordingId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PubliclyAccessible = table.Column<ulong>(type: "bit(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaRecordingReferences_GameItemComponents",
                        column: x => x.GameItemComponentId,
                        principalTable: "GameItemComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaRecordingReferences_MediaRecordings",
                        column: x => x.MediaRecordingId,
                        principalTable: "MediaRecordings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MediaRecordingFrames",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MediaRecordingId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MediaSceneSnapshotId = table.Column<long>(type: "bigint(20)", nullable: false),
                    StartOffsetMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false),
                    EndOffsetMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaRecordingFrames_MediaRecordings",
                        column: x => x.MediaRecordingId,
                        principalTable: "MediaRecordings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaRecordingFrames_MediaSceneSnapshots",
                        column: x => x.MediaSceneSnapshotId,
                        principalTable: "MediaSceneSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "UX_MediaRecordingChunks_Recording_Sequence",
                table: "MediaRecordingChunks",
                columns: new[] { "MediaRecordingId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_MediaRecordingFrames_MediaSceneSnapshots_idx",
                table: "MediaRecordingFrames",
                column: "MediaSceneSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaRecordingFrames_Recording_Offset",
                table: "MediaRecordingFrames",
                columns: new[] { "MediaRecordingId", "StartOffsetMilliseconds" });

            migrationBuilder.CreateIndex(
                name: "FK_MediaRecordingReferences_MediaRecordings_idx",
                table: "MediaRecordingReferences",
                column: "MediaRecordingId");

            migrationBuilder.CreateIndex(
                name: "UX_MediaRecordingReferences_Component_Name",
                table: "MediaRecordingReferences",
                columns: new[] { "GameItemComponentId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaRecordings_Status_Created",
                table: "MediaRecordings",
                columns: new[] { "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "UX_MediaSceneSnapshots_ContentHash",
                table: "MediaSceneSnapshots",
                column: "ContentHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaRecordingChunks");

            migrationBuilder.DropTable(
                name: "MediaRecordingFrames");

            migrationBuilder.DropTable(
                name: "MediaRecordingReferences");

            migrationBuilder.DropTable(
                name: "MediaSceneSnapshots");

            migrationBuilder.DropTable(
                name: "MediaRecordings");
        }
    }
}
