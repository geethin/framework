using AutoMapper;
using Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Share.Models.Common;

namespace App.Api.Controllers
{
    /// <summary>
    /// 授权登录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration _config;
        ContextBase _context;
        IMapper _mapper;
        public AuthController(
            IConfiguration configuration,
            ContextBase context,
            IMapper mapper
            )
        {
            _config = configuration;
            _context = context;
            _mapper = mapper;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("signIn")]
        public ActionResult<SignInDto> SignUp([FromBody] SignInForm dto)
        {
            return default;
            // 登录授权示例
            /*var user = _context.Account.Where(u => u.Email.Equals(dto.Username)
                 || u.Username.Equals(dto.Username))
                 .FirstOrDefault();
            if (user == null)
            {
                return NotFound("用户名或密码错误");
            }
            if (!HashCrypto.Validate(dto.Password, user.HashSalt, user.Password))
            {
                return Problem("用户名或密码错误");
            }
            var jwt = new JwtService();
            var issuerSign = _config.GetSection("Jwt")["Sign"];
            var issuer = _config.GetSection("Jwt")["Issuer"];
            var audience = _config.GetSection("Jwt")["Audience"];

            var roles = string.Join(";", user.Roles.Select(r => r.Name).ToList());
            var token = jwt.BuildToken(user.Id.ToString(), roles, issuerSign, audience, issuer);
            var result = new SignInDto
            {
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar,
                CreatedTime = user.CreatedTime,
                Id = user.Id,
                Token = token,
                RoleName = roles
            };
            return result;*/
        }

    }
}
