using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class SurgeryBodyUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TargetBodyTypeId",
                table: "SurgicalProcedures",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SurgicalProcedures_TargetBodyTypeId",
                table: "SurgicalProcedures",
                column: "TargetBodyTypeId");

            migrationBuilder.Sql(@"UPDATE SurgicalProcedures
SET TargetBodyTypeId = 1
WHERE Id > 0");

			migrationBuilder.AddForeignKey(
                name: "FK_SurgicalProcedures_BodyProtos",
                table: "SurgicalProcedures",
                column: "TargetBodyTypeId",
                principalTable: "BodyProtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurgicalProcedures_BodyProtos",
                table: "SurgicalProcedures");

            migrationBuilder.DropIndex(
                name: "IX_SurgicalProcedures_TargetBodyTypeId",
                table: "SurgicalProcedures");

            migrationBuilder.DropColumn(
                name: "TargetBodyTypeId",
                table: "SurgicalProcedures");
        }
    }
}
