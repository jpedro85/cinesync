using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfDeslikes",
                table: "Comments",
                newName: "NumberOfDislikes");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "Comments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "NumberOfDislikes",
                table: "Comments",
                newName: "NumberOfDeslikes");
        }
    }
}
