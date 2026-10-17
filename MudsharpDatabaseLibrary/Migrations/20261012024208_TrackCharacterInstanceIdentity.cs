using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class TrackCharacterInstanceIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CharacterInstanceId",
                table: "Tracks",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_Tracks_CharacterInstances_idx",
                table: "Tracks",
                column: "CharacterInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_CharacterInstances",
                table: "Tracks",
                column: "CharacterInstanceId",
                principalTable: "CharacterInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_CharacterInstances",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "FK_Tracks_CharacterInstances_idx",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "CharacterInstanceId",
                table: "Tracks");
        }
    }
}
