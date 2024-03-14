using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mumarket.Migrations
{
    /// <inheritdoc />
    public partial class AddRealmSell : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Realm",
                table: "Sells",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Realm",
                table: "Sells");
        }
    }
}
