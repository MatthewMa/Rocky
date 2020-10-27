using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky.Migrations
{
    public partial class addAppTypeToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppTypeId",
                table: "Products",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Products_AppTypeId",
                table: "Products",
                column: "AppTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppTypes_AppTypeId",
                table: "Products",
                column: "AppTypeId",
                principalTable: "AppTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppTypes_AppTypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_AppTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AppTypeId",
                table: "Products");
        }
    }
}
