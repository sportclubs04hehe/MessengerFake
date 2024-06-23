using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;

namespace MessengerFake.API.Service
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        Task<Photo?> GetPhotoById(Guid id);
        void RemovePhoto(Photo photo);
    }
}
