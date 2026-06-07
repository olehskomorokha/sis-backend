using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartInfluence.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Influencers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InfluencerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChannelUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lenguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FollowersCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Influencers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientInfluencers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    InfluencerId = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    BrandFitScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AiReview = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PredictedEngagement = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientInfluencers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientInfluencers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientInfluencers_Influencers_InfluencerId",
                        column: x => x.InfluencerId,
                        principalTable: "Influencers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InfluencerScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InfluencerId = table.Column<int>(type: "int", nullable: false),
                    EngagementRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PostsCount = table.Column<int>(type: "int", nullable: false),
                    AvgViews = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvgLikes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvgComments = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfluencerScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfluencerScores_Influencers_InfluencerId",
                        column: x => x.InfluencerId,
                        principalTable: "Influencers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientInfluencers_ClientId_InfluencerId",
                table: "ClientInfluencers",
                columns: new[] { "ClientId", "InfluencerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientInfluencers_InfluencerId",
                table: "ClientInfluencers",
                column: "InfluencerId");

            migrationBuilder.CreateIndex(
                name: "IX_Influencers_InfluencerId",
                table: "Influencers",
                column: "InfluencerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InfluencerScores_InfluencerId",
                table: "InfluencerScores",
                column: "InfluencerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientInfluencers");

            migrationBuilder.DropTable(
                name: "InfluencerScores");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Influencers");
        }
    }
}
