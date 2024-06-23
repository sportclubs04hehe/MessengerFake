namespace MessengerFake.API.Models.DTOs
{
    public class CreateMessageDto
    {
        public required string RecipientUsername { get; set; } // Tên người dùng người nhận
        public required string Content { get; set; } // Nội dung
    }
}
