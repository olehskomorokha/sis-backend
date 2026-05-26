using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartInfluence.Data.Migrations
{
    /// <inheritdoc />
    public partial class corrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Influencers_InfluencerScores_InfluencerScoreId",
                table: "Influencers");

            migrationBuilder.DropIndex(
                name: "IX_Influencers_InfluencerScoreId",
                table: "Influencers");

            migrationBuilder.DropColumn(
                name: "InfluencerScoreId",
                table: "Influencers");

            migrationBuilder.CreateIndex(
                name: "IX_InfluencerScores_InfluencerId",
                table: "InfluencerScores",
                column: "InfluencerId");

            migrationBuilder.AddForeignKey(
                name: "FK_InfluencerScores_Influencers_InfluencerId",
                table: "InfluencerScores",
                column: "InfluencerId",
                principalTable: "Influencers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InfluencerScores_Influencers_InfluencerId",
                table: "InfluencerScores");

            migrationBuilder.DropIndex(
                name: "IX_InfluencerScores_InfluencerId",
                table: "InfluencerScores");

            migrationBuilder.AddColumn<int>(
                name: "InfluencerScoreId",
                table: "Influencers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Influencers_InfluencerScoreId",
                table: "Influencers",
                column: "InfluencerScoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Influencers_InfluencerScores_InfluencerScoreId",
                table: "Influencers",
                column: "InfluencerScoreId",
                principalTable: "InfluencerScores",
                principalColumn: "Id");
        }
    }
}
