using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HealthCareBlog_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerUrlToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "saved_posts",
                newName: "saved_posts",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "reported_contents",
                newName: "reported_contents",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "posts",
                newName: "posts",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "nutrition_profile",
                newName: "nutrition_profile",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "nutrition_chat_session",
                newName: "nutrition_chat_session",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "nutrition_chat_message",
                newName: "nutrition_chat_message",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "notifications",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "like_posts",
                newName: "like_posts",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "like_comments",
                newName: "like_comments",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "follows",
                newName: "follows",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "comments",
                newName: "comments",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "public");

            migrationBuilder.AddColumn<string>(
                name: "image_urls",
                schema: "public",
                table: "posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "banner_url",
                schema: "public",
                table: "AspNetUsers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "meals",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name_en = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    meal_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    calories_per_serving = table.Column<int>(type: "integer", nullable: false),
                    protein_g = table.Column<decimal>(type: "numeric", nullable: false),
                    carbs_g = table.Column<decimal>(type: "numeric", nullable: false),
                    fat_g = table.Column<decimal>(type: "numeric", nullable: false),
                    serving_size_desc = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    suitable_for = table.Column<string[]>(type: "varchar[]", nullable: false),
                    tags = table.Column<string[]>(type: "varchar[]", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meals", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_meals_is_active",
                schema: "public",
                table: "meals",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_meals_meal_type",
                schema: "public",
                table: "meals",
                column: "meal_type");

            migrationBuilder.CreateIndex(
                name: "IX_meals_meal_type_is_active",
                schema: "public",
                table: "meals",
                columns: new[] { "meal_type", "is_active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "meals",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "image_urls",
                schema: "public",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "banner_url",
                schema: "public",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "saved_posts",
                schema: "public",
                newName: "saved_posts");

            migrationBuilder.RenameTable(
                name: "reported_contents",
                schema: "public",
                newName: "reported_contents");

            migrationBuilder.RenameTable(
                name: "posts",
                schema: "public",
                newName: "posts");

            migrationBuilder.RenameTable(
                name: "nutrition_profile",
                schema: "public",
                newName: "nutrition_profile");

            migrationBuilder.RenameTable(
                name: "nutrition_chat_session",
                schema: "public",
                newName: "nutrition_chat_session");

            migrationBuilder.RenameTable(
                name: "nutrition_chat_message",
                schema: "public",
                newName: "nutrition_chat_message");

            migrationBuilder.RenameTable(
                name: "notifications",
                schema: "public",
                newName: "notifications");

            migrationBuilder.RenameTable(
                name: "like_posts",
                schema: "public",
                newName: "like_posts");

            migrationBuilder.RenameTable(
                name: "like_comments",
                schema: "public",
                newName: "like_comments");

            migrationBuilder.RenameTable(
                name: "follows",
                schema: "public",
                newName: "follows");

            migrationBuilder.RenameTable(
                name: "comments",
                schema: "public",
                newName: "comments");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "public",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "public",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "public",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "public",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "public",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "public",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "public",
                newName: "AspNetRoleClaims");
        }
    }
}
