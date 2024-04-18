using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersNotifications_AspNetUsers_AplicationUserId",
                table: "UsersNotifications");

            migrationBuilder.DropIndex(
                name: "IX_UsersNotifications_AplicationUserId",
                table: "UsersNotifications");

            migrationBuilder.DropColumn(
                name: "AplicationUserId",
                table: "UsersNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "UsersNotifications",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_UsersNotifications_ApplicationUserId",
                table: "UsersNotifications",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersNotifications_AspNetUsers_ApplicationUserId",
                table: "UsersNotifications",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersNotifications_AspNetUsers_ApplicationUserId",
                table: "UsersNotifications");

            migrationBuilder.DropIndex(
                name: "IX_UsersNotifications_ApplicationUserId",
                table: "UsersNotifications");

            migrationBuilder.AlterColumn<uint>(
                name: "ApplicationUserId",
                table: "UsersNotifications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "AplicationUserId",
                table: "UsersNotifications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UsersNotifications_AplicationUserId",
                table: "UsersNotifications",
                column: "AplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersNotifications_AspNetUsers_AplicationUserId",
                table: "UsersNotifications",
                column: "AplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
