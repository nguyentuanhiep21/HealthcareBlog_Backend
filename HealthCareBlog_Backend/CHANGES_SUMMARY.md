# HealthCareBlog - C?p nh?t Ch?c N?ng

## ?? Tóm t?t các thay ??i m?i nh?t

### ? Tính n?ng GI? L?I:
1. **Like bình lu?n** ? Gi? (xóa Like bài vi?t ch?)
2. **Reply bình lu?n 1 c?p** ? Gi? (ParentCommentId, ch? 1 level)
3. **LikeCount trên Comment** ? Gi?

### ? Tính n?ng B? XÓA:
1. **Messaging** - Conversation, ConversationParticipant, Message
2. **Group** - Group, UserGroup, GroupRole, GroupJoinRequest, GroupPostPending
3. **PostShare** - Chia s? bài vi?t
4. **Hashtag** - Hashtag, PostHashtag (XÓA HOÀN TOÀN)

### ?? **Entity ???c s?a:**
- ?? **Like.cs** - Gi? CommentId, h? tr? like c? Post và Comment
- ?? **Comment.cs** - Gi? ParentCommentId (1 c?p), LikeCount, Replies
- ?? **Post.cs** - Xóa GroupId, PostShare, Hashtag references, xóa ShareCount
- ?? **ApplicationUser.cs** - Xóa PostShares, UserGroups, ConversationParticipants, SentMessages
- ?? **Notification.cs** - Xóa GroupId
- ?? **ApplicationDbContext.cs** - Xóa Hashtag/PostHashtag DbSets

### ?? Schema hi?n t?i:

**B?ng gi? l?i:**
- `AspNetUsers` - Ng??i dùng
- `Posts` - Bài vi?t
- `Comments` - Bình lu?n (v?i ParentCommentId cho reply)
- `Likes` - Like bài vi?t & bình lu?n
- `Follows` - Follow ng??i dùng
- `UserBlocks` - Ch?n ng??i dùng
- `Notifications` - Thông báo
- `HealthProfiles` - H? s? s?c kh?e
- `MealSuggestions` - G?i ý b?a ?n
- `AuditLogs` - Nh?t ký ki?m toán
- `ReportedContents` - Báo cáo n?i dung

**B?ng xóa:**
- ? Conversations, ConversationParticipants, Messages
- ? Groups, UserGroups, GroupRoles, GroupJoinRequests, GroupPostPendings
- ? PostShares
- ? Hashtags, PostHashtags

### ?? Migration:
- Migration: `RemoveMessagingGroupPostShareFeatures`
- Xóa Messaging, Group, PostShare, Hashtag tables
- Gi? l?i/Khôi ph?c Like cho Comments
- Gi? l?i ParentCommentId cho 1-level replies

### ? Status:
- **Build**: ? Thành công
- **All changes**: ? Hoàn thành
- **Ready for**: Database migration/update

## ?? Ti?p theo:
1. Ch?y migration: `dotnet ef database update`
2. C?p nh?t Controllers/Services
3. C?p nh?t DTOs n?u c?n
