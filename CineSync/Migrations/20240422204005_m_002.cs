using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "VoteCount",
                table: "Movies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoteCount",
                table: "Movies");
        }
    }
}
