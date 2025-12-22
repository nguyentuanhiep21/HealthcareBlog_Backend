using System.ComponentModel.DataAnnotations;

namespace HealthCareBlog_Backend.Models.Entities;

// Các enum dùng trong hệ thống để biểu diễn các giá trị rời rạc

public enum NotificationType
{
    [Display(Name = "Theo dõi")]
    Follow = 0,

    [Display(Name = "Thích")]
    Like = 1,

    [Display(Name = "Bình luận")]
    Comment = 2,
}

public enum AdminActionType
{
    [Display(Name = "Xóa bài viết")]
    DeletePost = 0,

    [Display(Name = "Xóa người dùng")]
    DeactivateUser = 1,

    [Display(Name = "Xóa bình luận")]
    DeleteComment = 2,

    [Display(Name = "Khóa người dùng")]
    BlockUser = 3,

    [Display(Name = "Gỡ khóa người dùng")]
    UnblockUser = 4,

    [Display(Name = "Xử lý báo cáo")]
    ResolveReport = 5,

    [Display(Name = "Từ chối báo cáo")]
    RejectReport = 6,
}

public enum UserStatus
{
    [Display(Name = "Hoạt động")]
    Active = 0,

    [Display(Name = "Bị khóa")]
    Deactivated = 1,
}

public enum ReportStatus
{
    [Display(Name = "Chưa xử lý")]
    Pending = 0,

    [Display(Name = "Đã xử lý")]
    Resolved = 1,

    [Display(Name = "Bị từ chối")]
    Rejected = 2,
}

public enum ContentType
{
    [Display(Name = "Bài viết")]
    Post = 0,

    [Display(Name = "Bình luận")]
    Comment = 1,

    [Display(Name = "Người dùng")]
    User = 2,
}

