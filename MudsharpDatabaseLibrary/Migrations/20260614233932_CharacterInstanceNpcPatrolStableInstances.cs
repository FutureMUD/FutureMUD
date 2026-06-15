using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CharacterInstanceNpcPatrolStableInstances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MountInstanceId",
                table: "StableStays",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PatrolLeaderInstanceId",
                table: "Patrols",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CharacterInstanceId",
                table: "PatrolMembers",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "PatrolMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "PatrolMembers",
                columns: new[] { "PatrolId", "CharacterId", "CharacterInstanceId" });

            migrationBuilder.CreateIndex(
                name: "FK_StableStays_CharacterInstances_Mount_idx",
                table: "StableStays",
                column: "MountInstanceId");

            migrationBuilder.CreateIndex(
                name: "FK_Patrols_CharacterInstances_Leader_idx",
                table: "Patrols",
                column: "PatrolLeaderInstanceId");

            migrationBuilder.CreateIndex(
                name: "FK_PatrolMembers_CharacterInstances_idx",
                table: "PatrolMembers",
                column: "CharacterInstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "FK_StableStays_CharacterInstances_Mount_idx",
                table: "StableStays");

            migrationBuilder.DropIndex(
                name: "FK_Patrols_CharacterInstances_Leader_idx",
                table: "Patrols");

            migrationBuilder.DropIndex(
                name: "FK_PatrolMembers_CharacterInstances_idx",
                table: "PatrolMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "PatrolMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "PatrolMembers",
                columns: new[] { "PatrolId", "CharacterId" });

            migrationBuilder.DropColumn(
                name: "MountInstanceId",
                table: "StableStays");

            migrationBuilder.DropColumn(
                name: "PatrolLeaderInstanceId",
                table: "Patrols");

            migrationBuilder.DropColumn(
                name: "CharacterInstanceId",
                table: "PatrolMembers");
        }
    }
}
