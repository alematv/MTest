using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MTest.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "search_queries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EngineName = table.Column<string>(nullable: false),
                    Query = table.Column<string>(nullable: false),
                    Time = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_search_queries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "search_results",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Link = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Position = table.Column<int>(nullable: true),
                    SearchQueryResultId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_search_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_search_results_search_queries_SearchQueryResultId",
                        column: x => x.SearchQueryResultId,
                        principalTable: "search_queries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_search_queries_Query",
                table: "search_queries",
                column: "Query");

            migrationBuilder.CreateIndex(
                name: "IX_search_results_Name",
                table: "search_results",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_search_results_SearchQueryResultId",
                table: "search_results",
                column: "SearchQueryResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "search_results");

            migrationBuilder.DropTable(
                name: "search_queries");
        }
    }
}
