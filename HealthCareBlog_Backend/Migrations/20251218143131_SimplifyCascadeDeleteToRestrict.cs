using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareBlog_Backend.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyCascadeDeleteToRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_posts_post_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_comments_comment_id",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_posts_post_id",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_AspNetUsers_user_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_AspNetUsers_user_id",
                table: "posts");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_posts_post_id",
                table: "comments",
                column: "post_id",
                principalTable: "posts",
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

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_AspNetUsers_user_id",
                table: "notifications",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_AspNetUsers_user_id",
                table: "posts",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_posts_post_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_comments_comment_id",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_posts_post_id",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_AspNetUsers_user_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_AspNetUsers_user_id",
                table: "posts");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_posts_post_id",
                table: "comments",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_likes_comments_comment_id",
                table: "likes",
                column: "comment_id",
                principalTable: "comments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_likes_posts_post_id",
                table: "likes",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_AspNetUsers_user_id",
                table: "notifications",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_AspNetUsers_user_id",
                table: "posts",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
