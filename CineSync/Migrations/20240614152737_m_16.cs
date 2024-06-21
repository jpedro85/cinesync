using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDislikedDiscussion",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscussionId = table.Column<uint>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDislikedDiscussion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDislikedDiscussion_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDislikedDiscussion_Discutions_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLikedDiscussion",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscussionId = table.Column<uint>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikedDiscussion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLikedDiscussion_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLikedDiscussion_Discutions_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDislikedDiscussion_DiscussionId",
                table: "UserDislikedDiscussion",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDislikedDiscussion_UserId",
                table: "UserDislikedDiscussion",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLikedDiscussion_DiscussionId",
                table: "UserLikedDiscussion",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLikedDiscussion_UserId",
                table: "UserLikedDiscussion",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDislikedDiscussion");

            migrationBuilder.DropTable(
                name: "UserLikedDiscussion");
        }
    }
}
