using System.ComponentModel.DataAnnotations;

namespace HealthCareBlog_Backend.Models.Entities;

// Các enum dùng trong hệ thống để biểu diễn các giá trị rời rạc

public enum Gender
{
    [Display(Name = "Nam")]
    Male = 0,

    [Display(Name = "Nữ")]
    Female = 1,

    [Display(Name = "Khác")]
    Other = 2
}

public enum NotificationType
{
    [Display(Name = "Theo dõi")]
    Follow = 0,

    [Display(Name = "Thích")]
    Like = 1,

    [Display(Name = "Bình luận")]
    Comment = 2,

    [Display(Name = "Phản hồi")]
    Reply = 3,

    [Display(Name = "Chia sẻ")]
    Share = 4,

    [Display(Name = "Mời vào nhóm")]
    GroupInvite = 5,

    [Display(Name = "Yêu cầu tham gia nhóm")]
    GroupJoinRequest = 6,

    [Display(Name = "Bài viết được duyệt")]
    GroupPostApproved = 7,

    [Display(Name = "Thành viên được duyệt")]
    GroupMemberApproved = 8,

    [Display(Name = "Nhắc đến")]
    Mention = 9
}

public enum ActivityLevel
{
    [Display(Name = "Ít vận động")]
    Sedentary = 0,

    [Display(Name = "Vận động nhẹ")]
    Light = 1,

    [Display(Name = "Vận động vừa")]
    Moderate = 2,

    [Display(Name = "Vận động nhiều")]
    Active = 3,

    [Display(Name = "Vận động rất nhiều")]
    VeryActive = 4
}

public enum AdminActionType
{
    [Display(Name = "Xóa bài viết")]
    DeletePost = 0,

    [Display(Name = "Khôi phục bài viết")]
    RestorePost = 1,

    [Display(Name = "Vô hiệu hóa người dùng")]
    DeactivateUser = 2,

    [Display(Name = "Kích hoạt người dùng")]
    ActivateUser = 3,

    [Display(Name = "Vô hiệu hóa nhóm")]
    DeactivateGroup = 4,

    [Display(Name = "Kích hoạt nhóm")]
    ActivateGroup = 5,

    [Display(Name = "Xóa bình luận")]
    DeleteComment = 6,

    [Display(Name = "Khôi phục bình luận")]
    RestoreComment = 7,

    [Display(Name = "Cấm người dùng vĩnh viễn")]
    BanUser = 8,

    [Display(Name = "Gỡ cấm người dùng")]
    UnbanUser = 9,

    [Display(Name = "Xử lý báo cáo")]
    ResolveReport = 10,

    [Display(Name = "Từ chối báo cáo")]
    RejectReport = 11,

    [Display(Name = "Gán vai trò Admin")]
    AssignAdminRole = 12,

    [Display(Name = "Gỡ vai trò Admin")]
    RemoveAdminRole = 13,

    [Display(Name = "Cập nhật nội dung")]
    UpdateContent = 14,

    [Display(Name = "Xóa vĩnh viễn")]
    PermanentDelete = 15
}

public enum UserStatus
{
    [Display(Name = "Hoạt động")]
    Active = 0,

    [Display(Name = "Vô hiệu hóa")]
    Deactivated = 1,

    [Display(Name = "Bị cấm")]
    Banned = 2,

    [Display(Name = "Đang chờ xác thực")]
    PendingVerification = 3
}