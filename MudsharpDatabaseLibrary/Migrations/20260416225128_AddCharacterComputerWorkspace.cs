using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterComputerWorkspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterComputerExecutables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerCharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OwnerHostItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OwnerStorageItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ExecutableKind = table.Column<int>(type: "int(11)", nullable: false),
                    CompilationContext = table.Column<int>(type: "int(11)", nullable: false),
                    ReturnTypeDefinition = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    SourceCode = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CompilationStatus = table.Column<int>(type: "int(11)", nullable: false),
                    CompileError = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AutorunOnBoot = table.Column<ulong>(type: "bit(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterComputerExecutables_Characters",
                        column: x => x.OwnerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterComputerExecutableParameters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterComputerExecutableId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ParameterIndex = table.Column<int>(type: "int(11)", nullable: false),
                    ParameterName = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ParameterTypeDefinition = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterComputerExecutableParameters_CharacterComputerExecutables",
                        column: x => x.CharacterComputerExecutableId,
                        principalTable: "CharacterComputerExecutables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterComputerProgramProcesses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterComputerExecutableId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OwnerCharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProcessName = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    WaitType = table.Column<int>(type: "int(11)", nullable: false),
                    WakeTimeUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    WaitArgument = table.Column<string>(type: "varchar(1000)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PowerLossBehaviour = table.Column<int>(type: "int(11)", nullable: false),
                    StateJson = table.Column<string>(type: "longtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ResultJson = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LastError = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndedAtUtc = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterComputerProgramProcesses_CharacterComputerExecutables",
                        column: x => x.CharacterComputerExecutableId,
                        principalTable: "CharacterComputerExecutables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterComputerProgramProcesses_Characters",
                        column: x => x.OwnerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterComputerExecutableParameters_Executables_idx",
                table: "CharacterComputerExecutableParameters",
                column: "CharacterComputerExecutableId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterComputerExecutableParameters_Executable_Index",
                table: "CharacterComputerExecutableParameters",
                columns: new[] { "CharacterComputerExecutableId", "ParameterIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_CharacterComputerExecutables_Characters_idx",
                table: "CharacterComputerExecutables",
                column: "OwnerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterComputerExecutables_Owner_Name",
                table: "CharacterComputerExecutables",
                columns: new[] { "OwnerCharacterId", "Name" });

            migrationBuilder.CreateIndex(
                name: "FK_CharacterComputerProgramProcesses_Characters_idx",
                table: "CharacterComputerProgramProcesses",
                column: "OwnerCharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterComputerProgramProcesses_Executables_idx",
                table: "CharacterComputerProgramProcesses",
                column: "CharacterComputerExecutableId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterComputerProgramProcesses_Owner_Status_Wake",
                table: "CharacterComputerProgramProcesses",
                columns: new[] { "OwnerCharacterId", "Status", "WakeTimeUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterComputerExecutableParameters");

            migrationBuilder.DropTable(
                name: "CharacterComputerProgramProcesses");

            migrationBuilder.DropTable(
                name: "CharacterComputerExecutables");
        }
    }
}
