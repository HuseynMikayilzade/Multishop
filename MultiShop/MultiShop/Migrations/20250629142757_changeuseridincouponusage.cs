using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiShop.Migrations
{
    /// <inheritdoc />
    public partial class changeuseridincouponusage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CouponsUsages_AspNetUsers_AppUserId",
                table: "CouponsUsages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CouponsUsages");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "CouponsUsages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CouponsUsages_AspNetUsers_AppUserId",
                table: "CouponsUsages",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CouponsUsages_AspNetUsers_AppUserId",
                table: "CouponsUsages");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "CouponsUsages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CouponsUsages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CouponsUsages_AspNetUsers_AppUserId",
                table: "CouponsUsages",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
