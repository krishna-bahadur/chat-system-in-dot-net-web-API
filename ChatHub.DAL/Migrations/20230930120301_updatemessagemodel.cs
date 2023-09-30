using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatHub.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatemessagemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileURL",
                table: "Messages");

            migrationBuilder.AddColumn<bool>(
                name: "IsFile",
                table: "Messages",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFile",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "FileURL",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
