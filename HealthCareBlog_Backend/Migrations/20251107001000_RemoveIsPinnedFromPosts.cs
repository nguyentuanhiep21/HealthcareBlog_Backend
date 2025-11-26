using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareBlog_Backend.Migrations
{
    public partial class RemoveIsPinnedFromPosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_pinned",
                table: "posts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_pinned",
                table: "posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
