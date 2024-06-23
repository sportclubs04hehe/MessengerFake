namespace MessengerFake.API.Models.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; } // đại diện cho người dùng gửi lượt thích
        public Guid SourceUserId { get; set; } // lưu trữ ID
        public AppUser TargetUser { get; set; } // đại diện cho người dùng nhận lượt thích
        public Guid TargetUserId { get; set; } // lưu trữ ID
    }
}