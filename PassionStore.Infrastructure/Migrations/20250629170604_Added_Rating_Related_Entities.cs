using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassionStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_Rating_Related_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HelpfulVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpfulVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HelpfulVotes_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HelpfulVotes_Ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Ratings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HelpfulVotes_RatingId_UserId",
                table: "HelpfulVotes",
                columns: new[] { "RatingId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HelpfulVotes_UserId",
                table: "HelpfulVotes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HelpfulVotes");
        }
    }
}
