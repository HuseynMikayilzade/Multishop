using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiShop.Migrations
{
    /// <inheritdoc />
    public partial class AddPropsforSpecialProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "SpecialProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "İmage",
                table: "SpecialProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "SpecialProducts");

            migrationBuilder.DropColumn(
                name: "İmage",
                table: "SpecialProducts");
        }
    }
}
