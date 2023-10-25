using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ScriptedEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScriptedEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CharacterFilterProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsReady = table.Column<ulong>(type: "bit(1)", nullable: false),
                    EarliestDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsFinished = table.Column<ulong>(type: "bit(1)", nullable: false),
                    IsTemplate = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptedEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptedEvents_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScriptedEvents_FutureProgs",
                        column: x => x.CharacterFilterProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ScriptedEventFreeTextQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScriptedEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Question = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Answer = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptedEventFreeTextQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptedEventFreeTextQuestions_ScriptedEvents",
                        column: x => x.ScriptedEventId,
                        principalTable: "ScriptedEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ScriptedEventMultipleChoiceQuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScriptedEventMultipleChoiceQuestionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DescriptionBeforeChoice = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DescriptionAfterChoice = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AnswerFilterProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AfterChoiceProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptedEventMultipleChoiceQuestionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_After",
                        column: x => x.AfterChoiceProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_Filter",
                        column: x => x.AnswerFilterProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ScriptedEventMultipleChoiceQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScriptedEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Question = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ChosenAnswerId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptedEventMultipleChoiceQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptedEventMultipleChoiceQuestions_ScriptedEventMultipleChoiceQuestionAnswers",
                        column: x => x.ChosenAnswerId,
                        principalTable: "ScriptedEventMultipleChoiceQuestionAnswers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScriptedEventMultipleChoiceQuestions_ScriptedEvents",
                        column: x => x.ScriptedEventId,
                        principalTable: "ScriptedEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptedEventFreeTextQuestions_ScriptedEventId",
                table: "ScriptedEventFreeTextQuestions",
                column: "ScriptedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptedEventMultipleChoiceQuestionAnswers_AfterChoiceProgId",
                table: "ScriptedEventMultipleChoiceQuestionAnswers",
                column: "AfterChoiceProgId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptedEventMultipleChoiceQuestionAnswers_AnswerFilterProgId",
                table: "ScriptedEventMultipleChoiceQuestionAnswers",
                column: "AnswerFilterProgId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMult~",
                table: "ScriptedEventMultipleChoiceQuestionAnswers",
                column: "ScriptedEventMultipleChoiceQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptedEventMultipleChoiceQuestions_ChosenAnswerId",
                table: "ScriptedEventMultipleChoiceQuestions",
                column: "ChosenAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptedEventMultipleChoiceQuestions_ScriptedEventId",
                table: "ScriptedEventMultipleChoiceQuestions",
                column: "ScriptedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptedEvents_CharacterFilterProgId",
                table: "ScriptedEvents",
                column: "CharacterFilterProgId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptedEvents_CharacterId",
                table: "ScriptedEvents",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMultipleChoiceQuestions",
                table: "ScriptedEventMultipleChoiceQuestionAnswers",
                column: "ScriptedEventMultipleChoiceQuestionId",
                principalTable: "ScriptedEventMultipleChoiceQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScriptedEventMultipleChoiceQuestions_ScriptedEvents",
                table: "ScriptedEventMultipleChoiceQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMultipleChoiceQuestions",
                table: "ScriptedEventMultipleChoiceQuestionAnswers");

            migrationBuilder.DropTable(
                name: "ScriptedEventFreeTextQuestions");

            migrationBuilder.DropTable(
                name: "ScriptedEvents");

            migrationBuilder.DropTable(
                name: "ScriptedEventMultipleChoiceQuestions");

            migrationBuilder.DropTable(
                name: "ScriptedEventMultipleChoiceQuestionAnswers");
        }
    }
}
