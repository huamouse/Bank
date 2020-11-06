using System;
using System.Threading.Tasks;
using CPTech.Models;
using CPTech.Payment.WeChatPay;
using CPTech.Payment.WeChatPay.Notify;
using CPTech.Payment.WeChatPay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HouseLease.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class WeChatController : ControllerBase
    {
        private readonly IWeChatPayClient client;
        private readonly WeChatPayOptions options;
        private readonly ILogger<WeChatController> logger;

        public WeChatController(IWeChatPayClient client, IOptions<WeChatPayOptions> options, ILogger<WeChatController> logger)
        {
            this.options = options.Value;
            this.client = client;
            this.logger = logger;
        }

        /// <summary>
        /// 获取OpenId
        /// </summary>
        [HttpGet("login/{jsCode}")]
        public async Task<ResultModel> LoginAsync(string jsCode)
        {
            var request = new WeChatLoginRequest
            {
                JsCode = jsCode
            };

            return ResultModel.Ok(await client.ExecuteAsync(request, options));
        }

        [HttpPost("renew")]
        public async Task<ResultModel> ContractPay((int ContractId, string OpenId) value)
        {
            throw new Exception();
            // 获取合同
            //var contract = await contractRepository.QueryContractAsync(value.ContractId);

            //var request = new WeChatPayUnifiedOrderRequest
            //{
            //    Body = contract.Address.LastIndexOf(",") == contract.Address.Length - 1 ? contract.Address[0..^1] : contract.Address,
            //    OutTradeNo = contract.Id.ToString() + DateTime.Now.ToString("HHmmss"),
            //    TotalFee = 1,//Convert.ToInt32(decimal.Multiply(contract.ContractRents.Sum(e => e.Amount), 100m)),
            //    SpBillCreateIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
            //    NotifyUrl = "https://cptech.com.cn/api/wechat/",
            //    TradeType = "JSAPI",
            //    OpenId = value.OpenId
            //};

            //var response = await client.ExecuteAsync(request, options);
            //if (response.ReturnCode != WeChatPayCode.Success || response.ResultCode != WeChatPayCode.Success)
            //    throw new NetException(response.ErrCodeDes);

            //var req = new WeChatPayLiteAppSdkRequest
            //{
            //    Package = "prepay_id=" + response.PrepayId
            //};
            //var parameter = await client.ExecuteAsync(req, options);

            //return ResultModel.Ok(parameter);
        }

        [HttpGet]
        public async Task<IActionResult> NotifyUrl()
        {
            var notify = await client.ExecuteAsync<WeChatPayUnifiedOrderNotify>(Request, options);
            if (notify.ReturnCode == WeChatPayCode.Success)
            {
                if (notify.ResultCode == WeChatPayCode.Success)
                {
                    Console.WriteLine("OutTradeNo: " + notify.OutTradeNo);

                    return WeChatPayNotifyResult.Success;
                }
            }
            return NoContent();
        }

        [HttpGet("qrcode")]
        public async Task<IActionResult> GetQrCode()
        {
            var notify = await client.ExecuteAsync<WeChatPayUnifiedOrderNotify>(Request, options);
            if (notify.ReturnCode == WeChatPayCode.Success)
            {
                if (notify.ResultCode == WeChatPayCode.Success)
                {
                    Console.WriteLine("OutTradeNo: " + notify.OutTradeNo);

                    return WeChatPayNotifyResult.Success;
                }
            }
            return NoContent();
        }
    }
}