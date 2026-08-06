using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ProjectQueueSchedulingAndLaunchEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLabourQueues_ActiveProjects",
                table: "ProjectLabourQueues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLabourQueues_ProjectLabourRequirements",
                table: "ProjectLabourQueues");

            migrationBuilder.AlterColumn<long>(
                name: "ProjectLabourRequirementId",
                table: "ProjectLabourQueues",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AlterColumn<long>(
                name: "ActiveProjectId",
                table: "ProjectLabourQueues",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AddColumn<long>(
                name: "ClaimingCharacterInstanceId",
                table: "ProjectLabourQueues",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletionMode",
                table: "ProjectLabourQueues",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ElapsedHours",
                table: "ProjectLabourQueues",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "EntryType",
                table: "ProjectLabourQueues",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LabourPreference",
                table: "ProjectLabourQueues",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "ProjectLabourQueues",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetHours",
                table: "ProjectLabourQueues",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<long>(
                name: "WatchedPhaseId",
                table: "ProjectLabourQueues",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ProjectLabourQueueLooping",
                table: "Characters",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "FK_ProjectLabourQueues_CharacterInstances_idx",
                table: "ProjectLabourQueues",
                column: "ClaimingCharacterInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLabourQueues_ActiveProjects",
                table: "ProjectLabourQueues",
                column: "ActiveProjectId",
                principalTable: "ActiveProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLabourQueues_CharacterInstances",
                table: "ProjectLabourQueues",
                column: "ClaimingCharacterInstanceId",
                principalTable: "CharacterInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLabourQueues_ProjectLabourRequirements",
                table: "ProjectLabourQueues",
                column: "ProjectLabourRequirementId",
                principalTable: "ProjectLabourRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLabourQueues_ActiveProjects",
                table: "ProjectLabourQueues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLabourQueues_CharacterInstances",
                table: "ProjectLabourQueues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLabourQueues_ProjectLabourRequirements",
                table: "ProjectLabourQueues");

            migrationBuilder.DropIndex(
                name: "FK_ProjectLabourQueues_CharacterInstances_idx",
                table: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "ClaimingCharacterInstanceId",
                table: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "CompletionMode",
                table: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "ElapsedHours",
                table: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "EntryType",
                table: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "LabourPreference",
                table: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "TargetHours",
                table: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "WatchedPhaseId",
                table: "ProjectLabourQueues");

            migrationBuilder.DropColumn(
                name: "ProjectLabourQueueLooping",
                table: "Characters");

            migrationBuilder.AlterColumn<long>(
                name: "ProjectLabourRequirementId",
                table: "ProjectLabourQueues",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ActiveProjectId",
                table: "ProjectLabourQueues",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLabourQueues_ActiveProjects",
                table: "ProjectLabourQueues",
                column: "ActiveProjectId",
                principalTable: "ActiveProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLabourQueues_ProjectLabourRequirements",
                table: "ProjectLabourQueues",
                column: "ProjectLabourRequirementId",
                principalTable: "ProjectLabourRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
