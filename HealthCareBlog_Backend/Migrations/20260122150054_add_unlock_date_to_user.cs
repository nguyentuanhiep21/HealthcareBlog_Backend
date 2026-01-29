using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareBlog_Backend.Migrations
{
    /// <inheritdoc />
    public partial class add_unlock_date_to_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "unlock_date",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "unlock_date",
                table: "AspNetUsers");
        }
    }
}
