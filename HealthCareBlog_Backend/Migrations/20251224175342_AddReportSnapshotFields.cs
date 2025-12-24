using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareBlog_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddReportSnapshotFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "target_content_snapshot",
                table: "reported_contents",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "target_user_fullname",
                table: "reported_contents",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "target_user_id",
                table: "reported_contents",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "target_content_snapshot",
                table: "reported_contents");

            migrationBuilder.DropColumn(
                name: "target_user_fullname",
                table: "reported_contents");

            migrationBuilder.DropColumn(
                name: "target_user_id",
                table: "reported_contents");
        }
    }
}
