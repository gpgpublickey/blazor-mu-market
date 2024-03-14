using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mumarket.Migrations
{
    /// <inheritdoc />
    public partial class ImgProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "Sells",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Img",
                table: "Sells");
        }
    }
}
