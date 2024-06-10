using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDislikedComments",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommentId = table.Column<uint>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDislikedComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDislikedComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDislikedComments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLikedComments",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommentId = table.Column<uint>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikedComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLikedComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLikedComments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDislikedComments_CommentId",
                table: "UserDislikedComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDislikedComments_UserId",
                table: "UserDislikedComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLikedComments_CommentId",
                table: "UserLikedComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLikedComments_UserId",
                table: "UserLikedComments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDislikedComments");

            migrationBuilder.DropTable(
                name: "UserLikedComments");
        }
    }
}
