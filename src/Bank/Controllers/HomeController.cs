using System;
using System.Threading.Tasks;
using CPTech.Core;
using CPTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly SqlDbContext dbContext;
        //private readonly IUserRepository userRepository;

        public HomeController(ILogger<HomeController> logger
            //, SqlDbContext dbContext
            //, IUserRepository userRepository
            )
        {
            _logger = logger;
            //this.dbContext = dbContext;
            //this.userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ResultModel> GetUserAsync()
        {
            throw new Exception("ddd");
            //return ResultModel.Ok();
        }

        [HttpGet("checkUser/{mobile}")]
        public bool checkUser(string mobile)
        {
            return false;//userRepository.CheckUser(mobile);
        }

        [HttpGet("user")]
        public async Task<ResultModel> EFCoreUser()
        {
            throw new Exception("");
            //return ResultModel.Ok(await dbContext.TUser.FirstOrDefaultAsync(e => e.Name != null));
        }

        [HttpGet("exception")]
        public string Exception()
        {
            throw new NetException(500, "统一异常中间件");
        }
    }
}