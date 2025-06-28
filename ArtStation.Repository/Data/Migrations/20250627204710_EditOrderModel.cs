using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtStation.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Shippings_ShippingId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ShippingId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "CustomerEmail",
                table: "Order",
                newName: "CustomerPhone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerPhone",
                table: "Order",
                newName: "CustomerEmail");

            migrationBuilder.AddColumn<int>(
                name: "ShippingId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShippingId",
                table: "Order",
                column: "ShippingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Shippings_ShippingId",
                table: "Order",
                column: "ShippingId",
                principalTable: "Shippings",
                principalColumn: "Id");
        }
    }
}
