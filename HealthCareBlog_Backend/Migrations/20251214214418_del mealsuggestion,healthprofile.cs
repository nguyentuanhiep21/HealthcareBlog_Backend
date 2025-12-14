using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareBlog_Backend.Migrations
{
    /// <inheritdoc />
    public partial class delmealsuggestionhealthprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_banned_by",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_deactivated_by",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "health_profiles");

            migrationBuilder.DropTable(
                name: "meal_suggestions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_banned_by",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_deactivated_by",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_user_name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ban_expires_at",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ban_reason",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "banned_at",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "banned_by",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "date_of_birth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "deactivated_at",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "deactivated_by",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "deactivation_reason",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "full_name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "height",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "profile_picture",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "status",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "weight",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "AspNetUsers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "AspNetUsers",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "AspNetUsers",
                newName: "PhoneNumber");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "AspNetUsers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "AspNetUsers",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "AspNetUsers",
                newName: "phone_number");

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ban_expires_at",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ban_reason",
                table: "AspNetUsers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "banned_at",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "banned_by",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bio",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_of_birth",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deactivated_at",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deactivated_by",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deactivation_reason",
                table: "AspNetUsers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "gender",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "height",
                table: "AspNetUsers",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "profile_picture",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "weight",
                table: "AspNetUsers",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "health_profiles",
                columns: table => new
                {
                    health_profile_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    activity_level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    age = table.Column<int>(type: "int", nullable: false),
                    bmi = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    dietary_restrictions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    height = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    weight = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_health_profiles", x => x.health_profile_id);
                    table.ForeignKey(
                        name: "FK_health_profiles_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "meal_suggestions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ai_model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ai_response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    activity_level = table.Column<int>(type: "int", nullable: true),
                    age = table.Column<int>(type: "int", nullable: false),
                    allergies = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    bmi = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    calories_recommendation = table.Column<int>(type: "int", nullable: true),
                    carbs_grams = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dietary_restrictions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    fat_grams = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    gender = table.Column<int>(type: "int", nullable: false),
                    health_goal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    height = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    is_saved = table.Column<bool>(type: "bit", nullable: false),
                    meal_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    protein_grams = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meal_suggestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_meal_suggestions_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_banned_by",
                table: "AspNetUsers",
                column: "banned_by");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_deactivated_by",
                table: "AspNetUsers",
                column: "deactivated_by");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_email",
                table: "AspNetUsers",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_user_name",
                table: "AspNetUsers",
                column: "user_name",
                unique: true,
                filter: "[user_name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_health_profiles_user_id",
                table: "health_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_meal_suggestions_user_id",
                table: "meal_suggestions",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_banned_by",
                table: "AspNetUsers",
                column: "banned_by",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_deactivated_by",
                table: "AspNetUsers",
                column: "deactivated_by",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
