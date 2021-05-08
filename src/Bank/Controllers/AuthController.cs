using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Bank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("nopermission")]
        public IActionResult NoPermission()
        {
            return Forbid("No Permission!");
        }

        /// <summary>
        /// login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Get((string Username, string Password) req)
        {
            if (CheckAccount(req.Username, req.Password, out string role))
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.NameIdentifier, req.Username),
                    new Claim("Role", role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    //颁发者
                    issuer: configuration["Authentication:JwtBearer:Issuer"],
                    //接收者
                    audience: configuration["Authentication:JwtBearer:Audience"],
                    //过期时间
                    expires: DateTime.Now.AddMinutes(30),
                    //签名证书
                    signingCredentials: creds,
                    //自定义参数
                    claims: claims
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return BadRequest(new { message = "username or password is incorrect." });
            }
        }

        /// <summary>
        /// 登陆校验
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private bool CheckAccount(string userName, string pwd, out string role)
        {
            role = "BankUser";
            if (userName.Equals(configuration["BankUser:Username"]) && pwd.Equals(configuration["BankUser:Password"]))
                return true;

            return false;
        }

        [HttpGet]
        [Authorize]
        [Route("login")]
        public IActionResult Login()
        {
            return Ok("Login!");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("version")]
        public IActionResult Version()
        {
            return Ok($"From {RuntimeInformation.FrameworkDescription}");
        }
    }
}