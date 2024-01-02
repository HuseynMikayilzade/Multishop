using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiShop.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialProdcutsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecialProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tittle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTittle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Button = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialProducts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecialProducts");
        }
    }
}
