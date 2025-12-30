using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareBlog_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AIchat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NutritionProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BMI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BMIStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionChatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NutritionProfileId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionChatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionChatSessions_NutritionProfiles_NutritionProfileId",
                        column: x => x.NutritionProfileId,
                        principalTable: "NutritionProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParsedMealsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionChatMessages_NutritionChatSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "NutritionChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NutritionChatMessages_CreatedAt",
                table: "NutritionChatMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionChatMessages_SessionId",
                table: "NutritionChatMessages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionChatSessions_CreatedAt",
                table: "NutritionChatSessions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionChatSessions_NutritionProfileId",
                table: "NutritionChatSessions",
                column: "NutritionProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NutritionProfiles_UserId",
                table: "NutritionProfiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NutritionChatMessages");

            migrationBuilder.DropTable(
                name: "NutritionChatSessions");

            migrationBuilder.DropTable(
                name: "NutritionProfiles");
        }
    }
}
