using CPTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HouseLease.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly JwtBearer jwtBearer;
        private readonly ILogger<UserController> logger;

        public UserController(IOptions<JwtBearer> jwtBearerOption, ILogger<UserController> logger)
        {
            this.jwtBearer = jwtBearerOption.Value;
            this.logger = logger;
        }

        [HttpPost("login")]
        public async Task<ResultModel> LoginAsync((string mobile, string password) userLogin)
        {
            // 这里校验用户名密码
            //ZcUser zcUser = await userRepository.LoginAsync(userLogin.mobile, userLogin.password) ?? throw new NetException(500, "用户名密码错误或等待管理员审核！");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userLogin.mobile),
                new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                new Claim(JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}")
            };

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"]));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtBearer.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                //颁发者
                //issuer: configuration["Authentication:JwtBearer:Issuer"],
                issuer: jwtBearer.Issuer,
                //接收者
                //audience: configuration["Authentication:JwtBearer:Audience"],
                audience: jwtBearer.Audience,
                //过期时间
                expires: DateTime.Now.AddMinutes(30),
                //签名证书
                signingCredentials: creds,
                //自定义参数
                claims: claims
                ); ;

            return ResultModel.Ok(new
            {
                Mobile = userLogin.mobile,
                //zcUser.ClientId,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            }); ;
        }
    }
}
