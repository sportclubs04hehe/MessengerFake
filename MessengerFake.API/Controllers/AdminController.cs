using MessengerFake.API.Models.Entities;
using MessengerFake.API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessengerFake.API.Controllers
{
    public class AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork,
    IPhotoService photoService) : BaseApiController
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    x.Id,
                    Username = x.UserName,
                    Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
                }).ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, string roles)
        {
            #region Giải thích
            //Except() là một phương thức mạnh mẽ trong LINQ giúp bạn loại bỏ các phần tử chung giữa hai tập hợp
            #endregion
            if (string.IsNullOrEmpty(roles)) return BadRequest("Bạn phải chọn ít nhất một vai trò");

            var selectedRoles = roles.Split(",").ToArray();

            var user = await userManager.FindByNameAsync(username);

            if (user == null) return BadRequest("User not found");

            var userRoles = await userManager.GetRolesAsync(user);

            // Thêm các vai trò mới mà người dùng chưa có
            // selectedRoles.Except(userRoles) trả về các vai trò có trong selectedRoles nhưng không có trong userRoles
            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            // Loại bỏ các vai trò mà người dùng hiện có nhưng không có trong danh sách vai trò mới
            // userRoles.Except(selectedRoles) trả về các vai trò có trong userRoles nhưng không có trong selectedRoles
            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();

            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId:guid}")]
        public async Task<ActionResult> ApprovePhoto([FromRoute]Guid photoId)
        {
            var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

            if (photo == null) return BadRequest("Ảnh này không tồn tại");

            photo.IsApproved = true;

            var user = await unitOfWork.UserRepository.GetUserByPhotoId(photoId);

            if (user == null) return BadRequest("Người dùng không tồn tại");

            if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

            await unitOfWork.Complete();

            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId:guid}")]
        public async Task<ActionResult> RejectPhoto(Guid photoId)
        {
            var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

            if (photo == null) return BadRequest("Ảnh này không tồn tại");

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Result == "ok".ToLower())
                {
                    unitOfWork.PhotoRepository.RemovePhoto(photo);
                }
            }
            else
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }

            await unitOfWork.Complete();

            return Ok();
        }
    }
}
