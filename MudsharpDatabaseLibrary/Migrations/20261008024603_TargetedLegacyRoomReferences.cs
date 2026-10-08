using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class TargetedLegacyRoomReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(MudSharp.Database.RoomReferenceMigrationInterceptor.Repair, suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			throw new System.NotSupportedException("Converted and preexisting Cell references are indistinguishable. Restore the verified database, binary and external files backup instead of reversing IDs.");
        }
    }
}
