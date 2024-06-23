using MessengerFake.API.Data;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessengerFake.API.Service.Impl
{
    public class PhotoRepository(DataContext _context) : IPhotoRepository
    {
        #region Giải thích
        //Phương thức IgnoreQueryFilters() cho phép bạn tạm thời bỏ qua các bộ lọc truy vấn toàn cục
        //đã được thiết lập trong DbContext.Điều này rất hữu ích khi bạn cần truy xuất dữ liệu mà
        //thông thường bị lọc ra bởi các bộ lọc này. Trong ví dụ trên, nó cho phép truy xuất cả những
        //sản phẩm đã ngừng kinh doanh, giúp quản trị viên có cái nhìn toàn diện về tất cả sản phẩm
        //trong cửa hàng.
        #endregion
        public async Task<Photo?> GetPhotoById(Guid id)
        {
            return await _context.Photos
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await _context.Photos
                .IgnoreQueryFilters()
                .Where(p => p.IsApproved == false)
                .Select(u => new PhotoForApprovalDto
                {
                    Id = u.Id,
                    Username = u.AppUser.UserName,
                    Url = u.Url,
                    IsApproved = u.IsApproved
                }).ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }


    }
}
