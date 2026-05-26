using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartInfluence.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeunnessecerytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfluencerTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropColumn(
                name: "AvgComments",
                table: "Influencers");

            migrationBuilder.DropColumn(
                name: "AvgLikes",
                table: "Influencers");

            migrationBuilder.DropColumn(
                name: "AvgViews",
                table: "Influencers");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TargetAudience",
                table: "Clients");

            migrationBuilder.AddColumn<decimal>(
                name: "AvgComments",
                table: "InfluencerScores",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AvgLikes",
                table: "InfluencerScores",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AvgViews",
                table: "InfluencerScores",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvgComments",
                table: "InfluencerScores");

            migrationBuilder.DropColumn(
                name: "AvgLikes",
                table: "InfluencerScores");

            migrationBuilder.DropColumn(
                name: "AvgViews",
                table: "InfluencerScores");

            migrationBuilder.AddColumn<decimal>(
                name: "AvgComments",
                table: "Influencers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AvgLikes",
                table: "Influencers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AvgViews",
                table: "Influencers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Clients",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetAudience",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelTagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideosTagName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InfluencerTags",
                columns: table => new
                {
                    InfluencerId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfluencerTags", x => new { x.InfluencerId, x.TagId });
                    table.ForeignKey(
                        name: "FK_InfluencerTags_Influencers_InfluencerId",
                        column: x => x.InfluencerId,
                        principalTable: "Influencers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InfluencerTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfluencerTags_TagId",
                table: "InfluencerTags",
                column: "TagId");
        }
    }
}
