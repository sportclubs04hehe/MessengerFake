using AutoMapper;
using MessengerFake.API.Extensions;
using MessengerFake.API.Helpers;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;
using MessengerFake.API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerFake.API.Controllers
{
    [Authorize]
    public class UserController(IUnitOfWork unitOfWork, IMapper mapper,
    IPhotoService photoService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            try
            {
                userParams.CurrentUsername = User.GetUsername();
                var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);

                Response.AddPaginationHeader(users);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("{username}")]  // /api/users/2
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            try
            {
                var currentUsername = User.GetUsername();

                var user = await unitOfWork.UserRepository.GetMemberAsync(username,
                isCurrentUser: currentUsername == username);

                if (user == null) return NotFound();

                return user;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            try
            {
                var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

                if (user == null) return BadRequest("Không tìm thấy người dùng");

                mapper.Map(memberUpdateDto, user);

                if (await unitOfWork.Complete()) return NoContent();

                return BadRequest("Lỗi khi cập nhật");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            try
            {
                var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

                if (user == null) return BadRequest("Không tìm thấy người dùng");

                var result = await photoService.AddPhotoAsync(file);

                if (result.Error != null) return BadRequest(result.Error.Message);

                var photo = new Photo
                {
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId
                };

                if(user.Photos.Count == 0) photo.IsMain = true;

                user.Photos.Add(photo);

                if (await unitOfWork.Complete())
                    return CreatedAtAction(nameof(GetUser),
                        new { username = user.UserName }, mapper.Map<PhotoDto>(photo));

                return BadRequest("Problem adding photo");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut("set-main-photo/{photoId:guid}")]
        public async Task<ActionResult> SetMainPhoto(Guid photoId)
        {
            try
            {
                var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

                if (user == null) return BadRequest("Không tìm thấy người dùng");

                var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

                if (photo == null || photo.IsMain) return BadRequest("Ảnh này đã là ảnh đại diện");

                var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                if (currentMain != null) currentMain.IsMain = false;

                photo.IsMain = true;

                if (await unitOfWork.Complete()) return NoContent();

                return BadRequest("Có vấn đề xảy ra khi cập nhật ảnh đại diện");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("delete-photo/{photoId:guid}")]
        public async Task<ActionResult> DeletePhoto(Guid photoId)
        {
            try
            {
                var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

                if (user == null) return BadRequest("User not found");

                var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

                if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");

                if (photo.PublicId != null)
                {
                    var result = await photoService.DeletePhotoAsync(photo.PublicId);
                    if (result.Error != null) return BadRequest(result.Error.Message);
                }

                user.Photos.Remove(photo);

                if (await unitOfWork.Complete()) return Ok();

                return BadRequest("Có vấn đề xảy ra khi xóa ảnh");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
