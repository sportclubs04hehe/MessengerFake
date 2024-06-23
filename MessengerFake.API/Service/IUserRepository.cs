using MessengerFake.API.Data;
using MessengerFake.API.Helpers;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;

namespace MessengerFake.API.Service
{
    public interface IUserRepository
    {
        #region Giải thích
        /*
         * ? có thể trả về một đối tượng AppUser hoặc null**/
        #endregion

        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser?> GetUserByIdAsync(Guid id);
        Task<AppUser?> GetUserByUsernameAsync(string username);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto?> GetMemberAsync(string username, bool isCurrentUser);
        Task<AppUser?> GetUserByPhotoId(Guid photoId);
    }
}
