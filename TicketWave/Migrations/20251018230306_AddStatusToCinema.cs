using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmPass.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToCinema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "cinemas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "cinemas");
        }
    }
}
