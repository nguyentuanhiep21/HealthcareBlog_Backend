using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareBlog_Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMessagingGroupPostShareFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_posts_groups_group_id",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_user_groups_group_roles_role_id",
                table: "user_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_user_groups_groups_group_id",
                table: "user_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_group_join_requests_groups_group_id",
                table: "group_join_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_group_post_pendings_groups_group_id",
                table: "group_post_pendings");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_conversations_conversation_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_conversation_participants_conversations_conversation_id",
                table: "conversation_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_post_shares_groups_group_id",
                table: "post_shares");

            migrationBuilder.DropForeignKey(
                name: "FK_post_hashtags_hashtags_hashtag_id",
                table: "post_hashtags");

            migrationBuilder.DropForeignKey(
                name: "FK_post_hashtags_posts_post_id",
                table: "post_hashtags");

            // Drop tables
            migrationBuilder.DropTable(
                name: "post_shares");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "conversation_participants");

            migrationBuilder.DropTable(
                name: "group_post_pendings");

            migrationBuilder.DropTable(
                name: "group_join_requests");

            migrationBuilder.DropTable(
                name: "user_groups");

            migrationBuilder.DropTable(
                name: "post_hashtags");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "group_roles");

            migrationBuilder.DropTable(
                name: "hashtags");

            migrationBuilder.DropTable(
                name: "groups");

            // Drop columns and indexes
            migrationBuilder.DropIndex(
                name: "IX_posts_group_id",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_group_id_created_at",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "share_count",
                table: "posts");

            // Update Like config to support both posts and comments
            migrationBuilder.AlterColumn<int>(
                name: "post_id",
                table: "likes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: false);

            migrationBuilder.DropIndex(
                name: "IX_likes_user_id_post_id",
                table: "likes");

            migrationBuilder.AddColumn<int>(
                name: "comment_id",
                table: "likes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_likes_comment_id",
                table: "likes",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_likes_post_id",
                table: "likes",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_likes_user_id_post_id_comment_id",
                table: "likes",
                columns: new[] { "user_id", "post_id", "comment_id" },
                unique: true,
                filter: "[post_id] IS NOT NULL AND [comment_id] IS NOT NULL");

            // Add parent_comment_id back to comments if it was removed
            migrationBuilder.AddColumn<int>(
                name: "parent_comment_id",
                table: "comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "like_count",
                table: "comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_comments_parent_comment_id",
                table: "comments",
                column: "parent_comment_id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_parent_comment_id",
                table: "comments",
                column: "parent_comment_id",
                principalTable: "comments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_likes_comments_comment_id",
                table: "likes",
                column: "comment_id",
                principalTable: "comments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_likes_posts_post_id",
                table: "likes",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This migration removes and modifies features significantly
            // Partial down operation would be complex and not fully reversible
            throw new NotImplementedException("This migration cannot be reversed");
        }
    }
}
