using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class DefaultFormTransformationEchoNonSelf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE StaticStrings
SET Text = '@ transform|transforms into ^1.'
WHERE Id = 'DefaultFormTransformationEcho'
AND Text = '@ transform|transforms into $1.';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE StaticStrings
SET Text = '@ transform|transforms into $1.'
WHERE Id = 'DefaultFormTransformationEcho'
AND Text = '@ transform|transforms into ^1.';");
        }
    }
}
