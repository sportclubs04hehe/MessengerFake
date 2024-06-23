using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MessengerFake.API.Helpers;
using Microsoft.Extensions.Options;

namespace MessengerFake.API.Service.Impl
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);

            _cloudinary = new Cloudinary(account); // Khởi tạo dịch vụ Cloudinary với tài khoản đã tạo.
        }

        /// <summary>
        /// Tải ảnh lên Cloudinary.
        /// </summary>
        /// <param name="file">Tệp ảnh để tải lên.</param>
        /// <returns>Kết quả của thao tác upload ảnh.</returns>
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            if (file == null || file.Length <= 0) // Kiểm tra xem file rỗng hay rỗng.
            {
                // Xử lý đầu vào không hợp lệ, ví dụ: ném ngoại lệ hoặc trả về kết quả thích hợp.
                return new ImageUploadResult { Error = new Error { Message = "Invalid file" } };
            }

            using var stream = file.OpenReadStream(); // Mở một luồng để đọc nội dung file.
            var uploadParams = new ImageUploadParams // thiết lập thông số upload file
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Crop("fill").Gravity("face"),
                Folder = "da-net7",
            };

            try
            {
                return await _cloudinary.UploadAsync(uploadParams); // thực hiện upload image and return result.
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, ví dụ: ghi nhật ký, trả về kết quả phù hợp hoặc thử lại.
                return new ImageUploadResult { Error = new Error { Message = "Upload failed" + ex } };
            }
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
