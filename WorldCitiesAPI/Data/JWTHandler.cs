using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Data
{
    public class JWTHandler
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public JWTHandler(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;

        }

        public async Task<JwtSecurityToken> GetTokenAsync(ApplicationUser user)
        {
            var jwtOptions = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["\"JwtSettings:audience"],
                claims: await GetClaimsAsync(user),
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(
                    _configuration["jwtSettings:ExpirationRimeInMinutes"])),
                signingCredentials: GetSigningCredentials());

            return jwtOptions;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:SecurityKey"]);
            var _secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(_secret,
                SecurityAlgorithms.HmacSha256);
        }


        private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
