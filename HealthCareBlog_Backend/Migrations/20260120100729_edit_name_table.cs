using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareBlog_Backend.Migrations
{
    /// <inheritdoc />
    public partial class edit_name_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NutritionChatMessages_NutritionChatSessions_SessionId",
                table: "NutritionChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_NutritionChatSessions_NutritionProfiles_NutritionProfileId",
                table: "NutritionChatSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_NutritionProfiles_AspNetUsers_UserId",
                table: "NutritionProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NutritionProfiles",
                table: "NutritionProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NutritionChatSessions",
                table: "NutritionChatSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NutritionChatMessages",
                table: "NutritionChatMessages");

            migrationBuilder.RenameTable(
                name: "NutritionProfiles",
                newName: "nutrition_profile");

            migrationBuilder.RenameTable(
                name: "NutritionChatSessions",
                newName: "nutrition_chat_session");

            migrationBuilder.RenameTable(
                name: "NutritionChatMessages",
                newName: "nutrition_chat_message");

            migrationBuilder.RenameIndex(
                name: "IX_NutritionProfiles_UserId",
                table: "nutrition_profile",
                newName: "IX_nutrition_profile_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_NutritionChatSessions_NutritionProfileId",
                table: "nutrition_chat_session",
                newName: "IX_nutrition_chat_session_NutritionProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_NutritionChatSessions_CreatedAt",
                table: "nutrition_chat_session",
                newName: "IX_nutrition_chat_session_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_NutritionChatMessages_SessionId",
                table: "nutrition_chat_message",
                newName: "IX_nutrition_chat_message_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_NutritionChatMessages_CreatedAt",
                table: "nutrition_chat_message",
                newName: "IX_nutrition_chat_message_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_nutrition_profile",
                table: "nutrition_profile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_nutrition_chat_session",
                table: "nutrition_chat_session",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_nutrition_chat_message",
                table: "nutrition_chat_message",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_nutrition_chat_message_nutrition_chat_session_SessionId",
                table: "nutrition_chat_message",
                column: "SessionId",
                principalTable: "nutrition_chat_session",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_nutrition_chat_session_nutrition_profile_NutritionProfileId",
                table: "nutrition_chat_session",
                column: "NutritionProfileId",
                principalTable: "nutrition_profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_nutrition_profile_AspNetUsers_UserId",
                table: "nutrition_profile",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nutrition_chat_message_nutrition_chat_session_SessionId",
                table: "nutrition_chat_message");

            migrationBuilder.DropForeignKey(
                name: "FK_nutrition_chat_session_nutrition_profile_NutritionProfileId",
                table: "nutrition_chat_session");

            migrationBuilder.DropForeignKey(
                name: "FK_nutrition_profile_AspNetUsers_UserId",
                table: "nutrition_profile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_nutrition_profile",
                table: "nutrition_profile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_nutrition_chat_session",
                table: "nutrition_chat_session");

            migrationBuilder.DropPrimaryKey(
                name: "PK_nutrition_chat_message",
                table: "nutrition_chat_message");

            migrationBuilder.RenameTable(
                name: "nutrition_profile",
                newName: "NutritionProfiles");

            migrationBuilder.RenameTable(
                name: "nutrition_chat_session",
                newName: "NutritionChatSessions");

            migrationBuilder.RenameTable(
                name: "nutrition_chat_message",
                newName: "NutritionChatMessages");

            migrationBuilder.RenameIndex(
                name: "IX_nutrition_profile_UserId",
                table: "NutritionProfiles",
                newName: "IX_NutritionProfiles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_nutrition_chat_session_NutritionProfileId",
                table: "NutritionChatSessions",
                newName: "IX_NutritionChatSessions_NutritionProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_nutrition_chat_session_CreatedAt",
                table: "NutritionChatSessions",
                newName: "IX_NutritionChatSessions_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_nutrition_chat_message_SessionId",
                table: "NutritionChatMessages",
                newName: "IX_NutritionChatMessages_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_nutrition_chat_message_CreatedAt",
                table: "NutritionChatMessages",
                newName: "IX_NutritionChatMessages_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NutritionProfiles",
                table: "NutritionProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NutritionChatSessions",
                table: "NutritionChatSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NutritionChatMessages",
                table: "NutritionChatMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NutritionChatMessages_NutritionChatSessions_SessionId",
                table: "NutritionChatMessages",
                column: "SessionId",
                principalTable: "NutritionChatSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NutritionChatSessions_NutritionProfiles_NutritionProfileId",
                table: "NutritionChatSessions",
                column: "NutritionProfileId",
                principalTable: "NutritionProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NutritionProfiles_AspNetUsers_UserId",
                table: "NutritionProfiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
