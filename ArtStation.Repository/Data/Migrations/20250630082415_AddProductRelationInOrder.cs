using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtStation.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductRelationInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ProductItem_ProductId",
                table: "OrderItem",
                column: "ProductItem_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Products_ProductItem_ProductId",
                table: "OrderItem",
                column: "ProductItem_ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Products_ProductItem_ProductId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_ProductItem_ProductId",
                table: "OrderItem");
        }
    }
}
