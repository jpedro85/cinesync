using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Hide",
                table: "Invites",
                newName: "HideByTarget");

            migrationBuilder.AddColumn<bool>(
                name: "HideBySender",
                table: "Invites",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HideBySender",
                table: "Invites");

            migrationBuilder.RenameColumn(
                name: "HideByTarget",
                table: "Invites",
                newName: "Hide");
        }
    }
}
