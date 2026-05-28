using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartInfluence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandFitScore",
                table: "InfluencerScores");

            migrationBuilder.DropColumn(
                name: "AiReview",
                table: "Influencers");

            migrationBuilder.DropColumn(
                name: "TargetCountry",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "EngagementScore",
                table: "InfluencerScores",
                newName: "EngagementRate");

            migrationBuilder.AddColumn<string>(
                name: "AiReview",
                table: "ClientInfluencers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "BrandFitScore",
                table: "ClientInfluencers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalScore",
                table: "ClientInfluencers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiReview",
                table: "ClientInfluencers");

            migrationBuilder.DropColumn(
                name: "BrandFitScore",
                table: "ClientInfluencers");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "ClientInfluencers");

            migrationBuilder.RenameColumn(
                name: "EngagementRate",
                table: "InfluencerScores",
                newName: "EngagementScore");

            migrationBuilder.AddColumn<decimal>(
                name: "BrandFitScore",
                table: "InfluencerScores",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AiReview",
                table: "Influencers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetCountry",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
