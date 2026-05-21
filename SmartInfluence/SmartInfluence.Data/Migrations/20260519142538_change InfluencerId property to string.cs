using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartInfluence.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeInfluencerIdpropertytostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InfluencerId",
                table: "Influencers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InfluencerId",
                table: "Influencers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
