using MessengerFake.API.Models.Entities;

namespace MessengerFake.API.Service
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
