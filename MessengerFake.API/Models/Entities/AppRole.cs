using Microsoft.AspNetCore.Identity;

namespace MessengerFake.API.Models.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
