using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSync.Migrations
{
    /// <inheritdoc />
    public partial class m_22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Envites",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConversationId = table.Column<uint>(type: "INTEGER", nullable: false),
                    CreatedTimestanp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    isGrouEnvite = table.Column<bool>(type: "INTEGER", nullable: false),
                    SenderId = table.Column<string>(type: "TEXT", nullable: true),
                    TragetId = table.Column<string>(type: "TEXT", nullable: true),
                    Accepted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Envites_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Envites_AspNetUsers_TragetId",
                        column: x => x.TragetId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Envites_SenderId",
                table: "Envites",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Envites_TragetId",
                table: "Envites",
                column: "TragetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Envites");
        }
    }
}
