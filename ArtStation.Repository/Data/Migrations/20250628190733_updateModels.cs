using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtStation.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredMaxDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeliveredMinDate",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "DeliveredOnAR",
                table: "Products",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveredOnEN",
                table: "Products",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ForWhom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAR = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForWhom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForWhomProduct",
                columns: table => new
                {
                    ForWhomOptionsId = table.Column<int>(type: "int", nullable: false),
                    productsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForWhomProduct", x => new { x.ForWhomOptionsId, x.productsId });
                    table.ForeignKey(
                        name: "FK_ForWhomProduct_ForWhom_ForWhomOptionsId",
                        column: x => x.ForWhomOptionsId,
                        principalTable: "ForWhom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForWhomProduct_Products_productsId",
                        column: x => x.productsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForWhomProduct_productsId",
                table: "ForWhomProduct",
                column: "productsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForWhomProduct");

            migrationBuilder.DropTable(
                name: "ForWhom");

            migrationBuilder.DropColumn(
                name: "DeliveredOnAR",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeliveredOnEN",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "DeliveredMaxDate",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveredMinDate",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
