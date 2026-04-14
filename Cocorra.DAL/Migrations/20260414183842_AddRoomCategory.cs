using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cocorra.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Rooms");
        }
    }
}
