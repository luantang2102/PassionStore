using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassionStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_Order_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentLink",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTransactionId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentLink",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentTransactionId",
                table: "Orders");
        }
    }
}
