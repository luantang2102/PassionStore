using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassionStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Order_Entity_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReturnReason",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnReason",
                table: "Orders");
        }
    }
}
