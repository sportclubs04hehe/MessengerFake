using AutoMapper;
using AutoMapper.QueryableExtensions;
using MessengerFake.API.Data;
using MessengerFake.API.Helpers;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessengerFake.API.Service.Impl
{
    public class LikesRepository(DataContext _context, IMapper _mapper) : ILikesRepository
    {
        public void AddLike(UserLike like)
        {
            _context.Likes.Add(like);
        }

        public void DeleteLike(UserLike like)
        {
            _context.Likes.Remove(like);
        }

        public async Task<IEnumerable<Guid>> GetCurrentUserLikeIds(Guid currentUserId)
        {
            return await _context.Likes
             .Where(x => x.SourceUserId == currentUserId)
             .Select(x => x.TargetUserId)
             .ToListAsync();
        }

        public async Task<UserLike?> GetUserLike(Guid sourceUserId, Guid targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
        {
            var likes = _context.Likes.AsQueryable();
            IQueryable<MemberDto> query;

            switch (likesParams.Predicate)
            {
                case "liked":
                    query = likes
                        .Where(x => x.SourceUserId == likesParams.UserId)
                        .Select(x => x.TargetUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;

                case "likedBy":
                    query = likes
                        .Where(x => x.TargetUserId == likesParams.UserId)
                        .Select(x => x.SourceUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;

                default:
                    var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);

                    query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;
            }

            return await PagedList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
        }

    }
}
