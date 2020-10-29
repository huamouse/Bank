using CPTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Bank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotifyController : ControllerBase
    {
        private readonly ILogger<NotifyController> logger;

        public NotifyController(ILogger<NotifyController> logger
            )
        {
            this.logger = logger;
        }

        [HttpPost("registrer")]
        public async Task<ResultModel> RegisterAsync((string Tag, string url) req)
        {
            logger.LogInformation($"RegisterAsync:{req}");
            return ResultModel.Ok("");
        }

        [HttpGet("registrer/{tag}")]
        public async Task<ResultModel> QueryAsync(string tag)
        {
            logger.LogInformation($"QueryAsync:{tag}");
            return ResultModel.Ok("");
        }

        [HttpGet("delete/{tag}")]
        public async Task<ResultModel> DeleteAsync(string tag)
        {
            logger.LogInformation($"DeleteAsync:{tag}");
            return ResultModel.Ok("");
        }
    }
}
