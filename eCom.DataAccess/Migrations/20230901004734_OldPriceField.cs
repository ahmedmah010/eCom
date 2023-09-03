using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OldPriceField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OldPrice",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldPrice",
                table: "Products");
        }
    }
}
