using System.Text.Json;
using System.Threading.Tasks;
using Bank.Domains.Payment;
using Bank.Domains.Payment.Entities;
using CPTech.Core;
using CPTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotifyController : ControllerBase
    {
        private readonly ILogger<NotifyController> logger;
        private readonly IPaymentRepository paymentRepository;

        public NotifyController(ILogger<NotifyController> logger,
            IPaymentRepository paymentRepository
            )
        {
            this.logger = logger;
            this.paymentRepository = paymentRepository;
        }

        [HttpPost("register")]
        public async Task<ResultModel> RegisterAsync((string Tag, string url, long? CreatorId) req)
        {
            logger.LogInformation($"RegisterAsync:{req}");
            var payNotify = await paymentRepository.SelectNotifyAsync(req.Tag);
            if (payNotify != null) ResultModel.Error(500, $"Tag: {req.Tag} has register！");

            int count = await paymentRepository.AddAsync(new PayNotify
            {
                Tag = req.Tag,
                Url = req.url,
                CreatorId = req.CreatorId
            });

            return count == 1 ? ResultModel.Ok("Notify Register Success！") : ResultModel.Error(500, "Notify Register Failed！");
        }

        [HttpGet("query/{tag}")]
        public async Task<ResultModel> QueryAsync(string tag)
        {
            logger.LogInformation($"QueryAsync:{tag}");
            var notify = await paymentRepository.SelectNotifyAsync(tag) ?? throw new NetException(500, "未找到有效的回调注册地址！");
            return ResultModel.Ok(JsonSerializer.Serialize(new
            {
                notify.Tag,
                notify.Url
            }, Constants.JsonSerializerOption));
        }

        [HttpGet("delete/{tag}")]
        public async Task<ResultModel> DeleteAsync(string tag)
        {
            logger.LogInformation($"DeleteAsync:{tag}");
            var notify = await paymentRepository.SelectNotifyAsync(tag) ?? throw new NetException(500, "未找到有效的回调注册地址！");
            notify.IsDeleted = true;
            int count = await paymentRepository.UpdateAsync(notify);
            return count == 1 ? ResultModel.Ok("Notify Delete Success！") : ResultModel.Error(500, "Notify Delete Failed！");
        }
    }
}