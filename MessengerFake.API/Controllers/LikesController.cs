using MessengerFake.API.Extensions;
using MessengerFake.API.Helpers;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;
using MessengerFake.API.Service;
using Microsoft.AspNetCore.Mvc;

namespace MessengerFake.API.Controllers
{
    public class LikesController(IUnitOfWork _unitOfWork) : BaseApiController
    {
        [HttpPost("{targetUserId:guid}")]
        public async Task<ActionResult> ToggleLike([FromRoute]Guid targetUserId)
        {
            try
            {
                var sourceUserId = User.GetUserId();

                if (sourceUserId == targetUserId) return BadRequest("Bạn không thể thích chính mình");

                var existingLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, targetUserId);

                if (existingLike == null)
                {
                    var like = new UserLike
                    {
                        SourceUserId = sourceUserId,
                        TargetUserId = targetUserId
                    };

                    _unitOfWork.LikesRepository.AddLike(like);
                }
                else
                {
                    _unitOfWork.LikesRepository.DeleteLike(existingLike);
                }

                if (await _unitOfWork.Complete()) return Ok();

                return BadRequest("Có lỗi xảy ra!!!");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Guid>>> GetCurrentUserLikeIds()
        {
            try
            {
                return Ok(await _unitOfWork.LikesRepository.GetCurrentUserLikeIds(User.GetUserId()));
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            try
            {
                likesParams.UserId = User.GetUserId();
                var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);

                Response.AddPaginationHeader(users);

                return Ok(users);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }

        }
    }
}
