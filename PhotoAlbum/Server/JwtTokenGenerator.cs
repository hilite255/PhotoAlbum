using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PhotoAlbum.Server
{
    public class JwtTokenGenerator
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SymmetricSecurityKey Key { get; set; }

        public object GenerateToken(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.Now.AddDays(1), credentials);
            return new { token = new JwtSecurityTokenHandler().WriteToken(token) };
        }
    }
}
