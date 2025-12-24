# File Management Logic

## Tổng quan

Backend đã được implement logic tự động xóa file ảnh vật lý khi:
- Xóa post
- Xóa user  
- Update post (thay ảnh hoặc xóa ảnh)
- Update avatar

## FileHelper Class

Tạo helper class tập trung logic xóa file:
- `FileHelper.DeleteFile(fileUrl)` - Xóa file bất kỳ
- `FileHelper.DeleteAvatar(avatarUrl)` - Xóa avatar (có check default)
- `FileHelper.DeletePostImage(imageUrl)` - Xóa ảnh post

**Location**: `HealthCareBlog_Backend/Helpers/FileHelper.cs`

## Các trường hợp xử lý

### 1. Xóa Post (`PostService.DeletePostAsync`)
```csharp
// Xóa ảnh trong uploads/posts/ nếu post có ảnh
FileHelper.DeletePostImage(post.ImageUrl);
// Sau đó xóa post trong database
```

### 2. Update Post (`PostService.UpdatePostAsync`)
```csharp
// Nếu ImageUrl thay đổi (thay ảnh mới hoặc xóa ảnh)
if (post.ImageUrl != updatePostDTO.ImageUrl)
{
    FileHelper.DeletePostImage(post.ImageUrl); // Xóa ảnh cũ
}
post.ImageUrl = updatePostDTO.ImageUrl; // Cập nhật URL mới (hoặc null)
```

**Các case được xử lý**:
- ✅ Thay ảnh mới → Xóa ảnh cũ, giữ ảnh mới
- ✅ Xóa ảnh (imageUrl = null) → Xóa ảnh cũ
- ✅ Giữ nguyên ảnh → Không xóa gì

### 3. Update Avatar (`UserService.UpdateAvatarAsync`)
```csharp
// Xóa avatar cũ (trừ default logo)
FileHelper.DeleteAvatar(user.AvatarUrl);
user.AvatarUrl = avatarUrl; // Cập nhật avatar mới
```

**Protection**: Không xóa avatar mặc định `/images/logo.png`

### 4. Xóa User (`UserService.DeleteUserAsync`)
```csharp
// Xóa avatar của user
FileHelper.DeleteAvatar(user.AvatarUrl);

// Xóa tất cả ảnh trong posts của user
foreach (var post in userPosts)
{
    FileHelper.DeletePostImage(post.ImageUrl);
}

// Sau đó xóa user và tất cả data liên quan
```

## Cấu trúc thư mục uploads

```
wwwroot/
  uploads/
    avatars/          # Avatar của users
      *.jpg, *.png
    posts/            # Ảnh trong bài viết
      *.jpg, *.png
```

**Note**: Thư mục `wwwroot/uploads/` đã được thêm vào `.gitignore`

## Error Handling

Tất cả operations xóa file đều có try-catch:
- Nếu file không tồn tại → Skip, không throw error
- Nếu có lỗi khi xóa → Log error, tiếp tục xử lý logic business
- Không block việc xóa database record nếu xóa file thất bại

## Testing Checklist

- [ ] Xóa post có ảnh → Kiểm tra file ảnh bị xóa
- [ ] Xóa post không có ảnh → Không có lỗi
- [ ] Update post: thay ảnh mới → Ảnh cũ bị xóa, ảnh mới còn
- [ ] Update post: xóa ảnh → Ảnh cũ bị xóa
- [ ] Update post: giữ nguyên → Ảnh không bị động
- [ ] Thay avatar → Avatar cũ bị xóa (trừ default)
- [ ] Xóa user → Avatar + tất cả ảnh posts bị xóa
