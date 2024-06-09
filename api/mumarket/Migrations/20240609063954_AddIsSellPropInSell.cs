using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mumarket.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSellPropInSell : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSell",
                table: "Sells",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSell",
                table: "Sells");
        }
    }
}
