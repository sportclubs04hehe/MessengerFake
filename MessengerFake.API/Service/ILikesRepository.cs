using MessengerFake.API.Helpers;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;

namespace MessengerFake.API.Service
{
    public interface ILikesRepository
    {
        Task<UserLike?> GetUserLike(Guid sourceUserId, Guid targetUserId);
        Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams);
        Task<IEnumerable<Guid>> GetCurrentUserLikeIds(Guid currentUserId);
        void DeleteLike(UserLike like);
        void AddLike(UserLike like);
    }
}
