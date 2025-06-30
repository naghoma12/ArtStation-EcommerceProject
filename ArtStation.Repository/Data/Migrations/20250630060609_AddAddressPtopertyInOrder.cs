using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtStation.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressPtopertyInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Order_AddressId",
                table: "Order",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Addresses_AddressId",
                table: "Order",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Addresses_AddressId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_AddressId",
                table: "Order");
        }
    }
}
