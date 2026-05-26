using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartInfluence.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientInfluencers_ClientId",
                table: "ClientInfluencers");

            migrationBuilder.AlterColumn<string>(
                name: "InfluencerId",
                table: "Influencers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Influencers_InfluencerId",
                table: "Influencers",
                column: "InfluencerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientInfluencers_ClientId_InfluencerId",
                table: "ClientInfluencers",
                columns: new[] { "ClientId", "InfluencerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Influencers_InfluencerId",
                table: "Influencers");

            migrationBuilder.DropIndex(
                name: "IX_ClientInfluencers_ClientId_InfluencerId",
                table: "ClientInfluencers");

            migrationBuilder.AlterColumn<string>(
                name: "InfluencerId",
                table: "Influencers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInfluencers_ClientId",
                table: "ClientInfluencers",
                column: "ClientId");
        }
    }
}
