using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassionStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Rating_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Helpful",
                table: "Ratings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Helpful",
                table: "Ratings");
        }
    }
}
