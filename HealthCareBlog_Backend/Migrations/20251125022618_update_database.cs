using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthCareBlog_Backend.Migrations
{
    /// <inheritdoc />
    public partial class update_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_groups_group_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_groups_group_id",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_reported_contents_posts_PostId",
                table: "reported_contents");

            migrationBuilder.DropTable(
                name: "conversation_participants");

            migrationBuilder.DropTable(
                name: "group_join_requests");

            migrationBuilder.DropTable(
                name: "group_post_pendings");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "post_hashtags");

            migrationBuilder.DropTable(
                name: "post_shares");

            migrationBuilder.DropTable(
                name: "user_groups");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "hashtags");

            migrationBuilder.DropTable(
                name: "group_roles");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropIndex(
                name: "IX_reported_contents_PostId",
                table: "reported_contents");

            migrationBuilder.DropIndex(
                name: "IX_posts_group_id",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_group_id_created_at",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_notifications_group_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_meal_suggestions_created_at",
                table: "meal_suggestions");

            migrationBuilder.DropIndex(
                name: "IX_meal_suggestions_user_id_created_at",
                table: "meal_suggestions");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "reported_contents");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "is_pinned",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "share_count",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "reply_count",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "ip_address",
                table: "audit_logs");

            migrationBuilder.AlterColumn<int>(
                name: "like_count",
                table: "posts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "comment_count",
                table: "posts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "meal_suggestions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "health_profiles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<int>(
                name: "like_count",
                table: "comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "reported_contents",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "like_count",
                table: "posts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "comment_count",
                table: "posts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "group_id",
                table: "posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_pinned",
                table: "posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "share_count",
                table: "posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "group_id",
                table: "notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "meal_suggestions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "health_profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "like_count",
                table: "comments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "reply_count",
                table: "comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ip_address",
                table: "audit_logs",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    avatar_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    type = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "group_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deactivated_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    deleted_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    owner_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    avatar_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    cover_image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    deactivated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deactivation_reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    member_count = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    post_count = table.Column<int>(type: "int", nullable: false),
                    privacy_type = table.Column<int>(type: "int", nullable: false),
                    require_member_approval = table.Column<bool>(type: "bit", nullable: false),
                    require_post_approval = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_groups_AspNetUsers_deactivated_by",
                        column: x => x.deactivated_by,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_groups_AspNetUsers_deleted_by",
                        column: x => x.deleted_by,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_groups_AspNetUsers_owner_id",
                        column: x => x.owner_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "hashtags",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    usage_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hashtags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "conversation_participants",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    conversation_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    joined_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    left_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversation_participants", x => x.id);
                    table.ForeignKey(
                        name: "FK_conversation_participants_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_conversation_participants_conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    conversation_id = table.Column<int>(type: "int", nullable: false),
                    sender_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_AspNetUsers_sender_id",
                        column: x => x.sender_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messages_conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_join_requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    responsed_by_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    user_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    is_approved = table.Column<bool>(type: "bit", nullable: false),
                    requested_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    responsed_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_join_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_group_join_requests_AspNetUsers_responsed_by_id",
                        column: x => x.responsed_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_group_join_requests_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_group_join_requests_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_post_pendings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    responsed_by_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    is_approved = table.Column<bool>(type: "bit", nullable: false),
                    responsed_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_post_pendings", x => x.id);
                    table.ForeignKey(
                        name: "FK_group_post_pendings_AspNetUsers_responsed_by_id",
                        column: x => x.responsed_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_group_post_pendings_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_post_pendings_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post_shares",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<int>(type: "int", nullable: true),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    share_content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_shares", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_shares_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_post_shares_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_post_shares_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    joined_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_groups_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_groups_group_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "group_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_groups_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_hashtags",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false),
                    hashtag_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_hashtags", x => new { x.post_id, x.hashtag_id });
                    table.ForeignKey(
                        name: "FK_post_hashtags_hashtags_hashtag_id",
                        column: x => x.hashtag_id,
                        principalTable: "hashtags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_hashtags_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "group_roles",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Chủ nhóm - Có toàn quyền quản lý nhóm", "Owner" },
                    { 2, "Quản trị viên - Quản lý thành viên và bài viết", "Admin" },
                    { 3, "Thành viên - Tham gia và đăng bài trong nhóm", "Member" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_reported_contents_PostId",
                table: "reported_contents",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_posts_group_id",
                table: "posts",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_group_id_created_at",
                table: "posts",
                columns: new[] { "group_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_group_id",
                table: "notifications",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_meal_suggestions_created_at",
                table: "meal_suggestions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_meal_suggestions_user_id_created_at",
                table: "meal_suggestions",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_conversation_participants_conversation_id_user_id",
                table: "conversation_participants",
                columns: new[] { "conversation_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_conversation_participants_user_id",
                table: "conversation_participants",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_conversations_created_at",
                table: "conversations",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_conversations_type",
                table: "conversations",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_group_join_requests_group_id_user_id_is_approved",
                table: "group_join_requests",
                columns: new[] { "group_id", "user_id", "is_approved" });

            migrationBuilder.CreateIndex(
                name: "IX_group_join_requests_requested_at",
                table: "group_join_requests",
                column: "requested_at");

            migrationBuilder.CreateIndex(
                name: "IX_group_join_requests_responsed_by_id",
                table: "group_join_requests",
                column: "responsed_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_join_requests_user_id",
                table: "group_join_requests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_post_pendings_group_id_is_approved",
                table: "group_post_pendings",
                columns: new[] { "group_id", "is_approved" });

            migrationBuilder.CreateIndex(
                name: "IX_group_post_pendings_post_id",
                table: "group_post_pendings",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_post_pendings_responsed_by_id",
                table: "group_post_pendings",
                column: "responsed_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_post_pendings_submitted_at",
                table: "group_post_pendings",
                column: "submitted_at");

            migrationBuilder.CreateIndex(
                name: "IX_group_roles_name",
                table: "group_roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_groups_created_at",
                table: "groups",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_groups_deactivated_by",
                table: "groups",
                column: "deactivated_by");

            migrationBuilder.CreateIndex(
                name: "IX_groups_deleted_by",
                table: "groups",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_groups_owner_id",
                table: "groups",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_status",
                table: "groups",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_hashtags_name",
                table: "hashtags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_conversation_id",
                table: "messages",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_conversation_id_created_at",
                table: "messages",
                columns: new[] { "conversation_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_created_at",
                table: "messages",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_hashtags_hashtag_id",
                table: "post_hashtags",
                column: "hashtag_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_shares_created_at",
                table: "post_shares",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_shares_group_id",
                table: "post_shares",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_shares_post_id",
                table: "post_shares",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_shares_user_id_post_id_created_at",
                table: "post_shares",
                columns: new[] { "user_id", "post_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_user_groups_group_id",
                table: "user_groups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_groups_role_id",
                table: "user_groups",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_groups_user_id_group_id",
                table: "user_groups",
                columns: new[] { "user_id", "group_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_groups_group_id",
                table: "notifications",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_groups_group_id",
                table: "posts",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reported_contents_posts_PostId",
                table: "reported_contents",
                column: "PostId",
                principalTable: "posts",
                principalColumn: "id");
        }
    }
}
