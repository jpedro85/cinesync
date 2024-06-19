using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discutions_Movies_MovieId",
                table: "Discutions");

            migrationBuilder.AlterColumn<uint>(
                name: "MovieId",
                table: "Discutions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(uint),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "Discutions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Discutions_Movies_MovieId",
                table: "Discutions",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discutions_Movies_MovieId",
                table: "Discutions");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Discutions");

            migrationBuilder.AlterColumn<uint>(
                name: "MovieId",
                table: "Discutions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(uint),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Discutions_Movies_MovieId",
                table: "Discutions",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");
        }
    }
}
