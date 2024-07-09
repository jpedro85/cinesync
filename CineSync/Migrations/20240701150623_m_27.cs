using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_AspNetUsers_TragetId",
                table: "Invites");

            migrationBuilder.RenameColumn(
                name: "TragetId",
                table: "Invites",
                newName: "TargetId");

            migrationBuilder.RenameIndex(
                name: "IX_Invites_TragetId",
                table: "Invites",
                newName: "IX_Invites_TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_AspNetUsers_TargetId",
                table: "Invites",
                column: "TargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_AspNetUsers_TargetId",
                table: "Invites");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                table: "Invites",
                newName: "TragetId");

            migrationBuilder.RenameIndex(
                name: "IX_Invites_TargetId",
                table: "Invites",
                newName: "IX_Invites_TragetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_AspNetUsers_TragetId",
                table: "Invites",
                column: "TragetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
