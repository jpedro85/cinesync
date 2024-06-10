using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_07 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageType",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageType",
                table: "AspNetUsers");
        }
    }
}
