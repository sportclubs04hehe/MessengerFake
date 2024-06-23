using MessengerFake.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessengerFake.API.Service.Impl
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

        }

        public async Task<string> CreateToken(AppUser user)
        {
            // Danh sách các claims chứa thông tin về người dùng
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            // Danh sách các vai trò của người dùng
            var roles = await _userManager.GetRolesAsync(user);

            // Thêm các claims cho từng vai trò của người dùng
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Tạo thông tin xác thực (credentials) để ký token
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Định danh chứa các claims
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds // Thông tin xác thực dùng để ký token.
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
