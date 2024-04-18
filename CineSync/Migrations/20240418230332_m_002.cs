using System;
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
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ApplicationUser_AutorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Collections_MovieCollectionId",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "ApplicationUserApplicationUser1");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_Movies_MovieCollectionId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "MovieCollectionId",
                table: "Movies");

            migrationBuilder.CreateTable(
                name: "CollectionsMovies",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MovieId = table.Column<uint>(type: "INTEGER", nullable: false),
                    MovieCollectionId = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionsMovies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionsMovies_Collections_MovieCollectionId",
                        column: x => x.MovieCollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionsMovies_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersNotifications",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NotificationId = table.Column<uint>(type: "INTEGER", nullable: false),
                    ApplicationUserId = table.Column<uint>(type: "INTEGER", nullable: false),
                    AplicationUserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersNotifications_AspNetUsers_AplicationUserId",
                        column: x => x.AplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersNotifications_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionsMovies_MovieCollectionId",
                table: "CollectionsMovies",
                column: "MovieCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionsMovies_MovieId",
                table: "CollectionsMovies",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersNotifications_AplicationUserId",
                table: "UsersNotifications",
                column: "AplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersNotifications_NotificationId",
                table: "UsersNotifications",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_AutorId",
                table: "Comments",
                column: "AutorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_AutorId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "CollectionsMovies");

            migrationBuilder.DropTable(
                name: "UsersNotifications");

            migrationBuilder.AddColumn<int>(
                name: "MovieCollectionId",
                table: "Movies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationId = table.Column<int>(type: "INTEGER", nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProfileImage = table.Column<byte[]>(type: "BLOB", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    WatchTime = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserApplicationUser1",
                columns: table => new
                {
                    FollowersId = table.Column<string>(type: "TEXT", nullable: false),
                    FollowingId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserApplicationUser1", x => new { x.FollowersId, x.FollowingId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserApplicationUser1_ApplicationUser_FollowersId",
                        column: x => x.FollowersId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserApplicationUser1_ApplicationUser_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_MovieCollectionId",
                table: "Movies",
                column: "MovieCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_NotificationId",
                table: "ApplicationUser",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserApplicationUser1_FollowingId",
                table: "ApplicationUserApplicationUser1",
                column: "FollowingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ApplicationUser_AutorId",
                table: "Comments",
                column: "AutorId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Collections_MovieCollectionId",
                table: "Movies",
                column: "MovieCollectionId",
                principalTable: "Collections",
                principalColumn: "Id");
        }
    }
}
