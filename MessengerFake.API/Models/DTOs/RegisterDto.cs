using System.ComponentModel.DataAnnotations;

namespace MessengerFake.API.Models.DTOs
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; }

        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }

        [Required] public DateOnly? DateOfBirth { get; set; } // Lưu ý điều này phải là tùy chọn nếu không trình xác thực bắt buộc sẽ không hoạt động
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
