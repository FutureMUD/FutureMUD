using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AIStorytellerScopedModelReasoning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttentionClassifierModel",
                table: "AIStorytellers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AttentionClassifierReasoningEffort",
                table: "AIStorytellers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TimeModel",
                table: "AIStorytellers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TimeReasoningEffort",
                table: "AIStorytellers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttentionClassifierModel",
                table: "AIStorytellers");

            migrationBuilder.DropColumn(
                name: "AttentionClassifierReasoningEffort",
                table: "AIStorytellers");

            migrationBuilder.DropColumn(
                name: "TimeModel",
                table: "AIStorytellers");

            migrationBuilder.DropColumn(
                name: "TimeReasoningEffort",
                table: "AIStorytellers");
        }
    }
}
