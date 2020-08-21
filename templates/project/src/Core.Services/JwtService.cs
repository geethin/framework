using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services
{
    public class JwtService
    {
        public JwtService()
        {
        }
        /// <summary>
        /// 生成jwt token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="sign"></param>
        /// <param name="audience"></param>
        /// <param name="issuer"></param>
        /// <returns></returns>
        public string BuildToken(string username, string sign, string audience, string issuer)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sign));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                // 此处自定义claims
                new Claim(ClaimTypes.Name, username),
            };
            var jwt = new JwtSecurityToken(issuer, audience, claims, signingCredentials: signingCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
    }
}
