using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_AspNetUsers_SenderId",
                table: "Invites");

            migrationBuilder.DropForeignKey(
                name: "FK_Invites_AspNetUsers_TragetId",
                table: "Invites");

            migrationBuilder.AlterColumn<string>(
                name: "TragetId",
                table: "Invites",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Invites",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_AspNetUsers_SenderId",
                table: "Invites",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_AspNetUsers_TragetId",
                table: "Invites",
                column: "TragetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_AspNetUsers_SenderId",
                table: "Invites");

            migrationBuilder.DropForeignKey(
                name: "FK_Invites_AspNetUsers_TragetId",
                table: "Invites");

            migrationBuilder.AlterColumn<string>(
                name: "TragetId",
                table: "Invites",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Invites",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_AspNetUsers_SenderId",
                table: "Invites",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_AspNetUsers_TragetId",
                table: "Invites",
                column: "TragetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
