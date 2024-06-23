using AutoMapper;
using AutoMapper.QueryableExtensions;
using MessengerFake.API.Data;
using MessengerFake.API.Helpers;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessengerFake.API.Service.Impl
{
    public class UserRepository(DataContext _context, IMapper _mapper) : IUserRepository
    {
        public async Task<MemberDto?> GetMemberAsync(string username, bool isCurrentUser)
        {
            var query = _context.Users
           .Where(x => x.UserName == username)
           .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
           .AsQueryable();

            if (isCurrentUser) query = query.IgnoreQueryFilters();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            // Loại trừ người dùng hiện tại
            query = query.Where(u => u.UserName != userParams.CurrentUsername);

            // Lọc theo giới tính
            query = query.Where(u => u.Gender == userParams.Gender);

            // Tính toán ngày sinh để lọc theo độ tuổi
            #region Giải thích
            /*
             * Ví dụ: Giả sử hôm nay là ngày 17/06/2024 và MinAge là 18. DateTime.Today.AddYears(-18) sẽ trả về ngày 17/06/2006.
             * Người sinh vào ngày 17/06/2006 sẽ đủ 18 tuổi vào ngày 17/06/2024.**/
            #endregion
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            // Lọc theo độ tuổi
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            // Sắp xếp kết quả
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            // Trả về danh sách phân trang
            return await PagedList<MemberDto>.CreateAsync(query.AsNoTracking()
           .ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
               userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<AppUser?> GetUserByPhotoId(Guid photoId)
        {
            // lấy user có ảnh khớp với  photoId truyền vào
            return await _context.Users
            .Include(p => p.Photos)
            .IgnoreQueryFilters()
            .Where(p => p.Photos.Any(p => p.Id == photoId))
            .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
