using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartInfluence.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientInfluencers_Clients_CampaignId",
                table: "ClientInfluencers");

            migrationBuilder.DropForeignKey(
                name: "FK_InfluencerScores_Influencers_InfluencerId",
                table: "InfluencerScores");

            migrationBuilder.DropIndex(
                name: "IX_InfluencerScores_InfluencerId",
                table: "InfluencerScores");

            migrationBuilder.RenameColumn(
                name: "CampaignId",
                table: "ClientInfluencers",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientInfluencers_CampaignId",
                table: "ClientInfluencers",
                newName: "IX_ClientInfluencers_ClientId");

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
                name: "FK_ClientInfluencers_Clients_ClientId",
                table: "ClientInfluencers",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Influencers_InfluencerScores_InfluencerScoreId",
                table: "Influencers",
                column: "InfluencerScoreId",
                principalTable: "InfluencerScores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientInfluencers_Clients_ClientId",
                table: "ClientInfluencers");

            migrationBuilder.DropForeignKey(
                name: "FK_Influencers_InfluencerScores_InfluencerScoreId",
                table: "Influencers");

            migrationBuilder.DropIndex(
                name: "IX_Influencers_InfluencerScoreId",
                table: "Influencers");

            migrationBuilder.DropColumn(
                name: "InfluencerScoreId",
                table: "Influencers");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "ClientInfluencers",
                newName: "CampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientInfluencers_ClientId",
                table: "ClientInfluencers",
                newName: "IX_ClientInfluencers_CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_InfluencerScores_InfluencerId",
                table: "InfluencerScores",
                column: "InfluencerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientInfluencers_Clients_CampaignId",
                table: "ClientInfluencers",
                column: "CampaignId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InfluencerScores_Influencers_InfluencerId",
                table: "InfluencerScores",
                column: "InfluencerId",
                principalTable: "Influencers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
