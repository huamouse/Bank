using Bank.Domains.Payment;
using Bank.EFCore;
using CPTech.Core;
using CPTech.Models;
using CPTech.Security;
using Icbc.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bank.Controllers
{
    [Route("api/pay")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> logger;
        private readonly SqlDbContext dbContext;
        private readonly IEnumerable<IPaymentService> paymentServices;
        private readonly string privateKey;

        public PaymentController(IConfiguration configuration,
            SqlDbContext dbContext,
            IEnumerable<IPaymentService> paymentServices,
            ILogger<PaymentController> logger)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.paymentServices = paymentServices;
            this.privateKey = configuration.GetValue<string>("Payment:ICBC:PrivateKey");
        }

        [HttpGet]
        public ResultModel CommTest()
        {
            return ResultModel.Ok("Comm test success!");
        }

        [HttpPost("orderCreate")]
        public async Task<ResultModel> OrderCreate(OrderCreateDto req)
        {
            logger.LogInformation($"create order req: {JsonSerializer.Serialize(req)}");

            PayOrder payOrder = new PayOrder()
            {
                Amount = decimal.ToInt32(req.Amount * 100),
                Note = req.Note,
                CreatorId = req.CreateId,
                CreationTime = DateTime.Now
            };
            dbContext.Add(payOrder);
            await dbContext.SaveChangesAsync();

            return ResultModel.Ok(new
            {
                OrderId = payOrder.Id
            });
        }

        [HttpPost("OrderPay")]
        public async Task<ResultModel> OrderPay(OrderPayDto req)
        {
            var paymentService = paymentServices.FirstOrDefault(service => service.GetType().Name.Equals(req.Channel + "Service", 
                StringComparison.OrdinalIgnoreCase)) ?? throw new NetException(500, $"未实现的通道服务:{req.Channel}Service");

            var payOrder = await dbContext.PayOrder.FindAsync(req.OrderId);
            payOrder.PayType = Enum.Parse<PayTypeEnum>(req.PayType, true);
            payOrder.Channel = Enum.Parse<ChannelEnum>(req.Channel, true);
            payOrder.Payer = req.Payer;

            string response = paymentService.Query(new PayQuery
            {
                PayType = payOrder.PayType,
                OrderNo = payOrder.Id.ToString(),
                QueryType = QueryTypeEnum.OrderQuery
            });
            return ResultModel.Ok(response);
        }

        [HttpPost("OrderQuery")]
        public async Task<ResultModel> OrderQuery(OrderQueryDto req)
        {
            Enum.TryParse<QueryTypeEnum>(req.QueryType, true, out QueryTypeEnum queryType);
            var payOrder = await dbContext.PayOrder.FindAsync(req.OrderId);
            PayTypeEnum payType = (PayTypeEnum)payOrder.PayType;
            ChannelEnum channel = (ChannelEnum)payOrder.Channel;

            var paymentService = paymentServices.FirstOrDefault(service => service.GetType().Name.Equals(channel + "Service",
                StringComparison.OrdinalIgnoreCase)) ?? throw new NetException(500, $"未实现的通道服务:{channel}Service");

            string response = paymentService.Query(new PayQuery
            {
                OrderNo = payOrder.Id.ToString(),
                PayType = payOrder.PayType,
                QueryType = queryType
            });
            return ResultModel.Ok(response);
        }

        [HttpPost("eventreceive")]
        public async Task<string> EventReceive([FromForm] B2cNotifyRequest req)
        {
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            Console.WriteLine("callback:" + HttpContext.Request.ContentType);
            Console.WriteLine($"callback req: {JsonSerializer.Serialize(req, serializerOptions)}");
            var bizContent = JsonSerializer.Deserialize<B2cNotifyRequest.BizContent>(req.Biz_content);

            // 更新数据库
            long id = long.Parse(bizContent.OutTradeNo);
            var payOrder = dbContext.PayOrder.Find(id);
            if (bizContent.ReturnCode == "0" && payOrder.Status != (int)PaymentStatus.Success)
                payOrder.Status = (int)PaymentStatus.Success;
            else if (bizContent.ReturnCode == "1" && payOrder.Status != (int)PaymentStatus.Fail)
            {
                payOrder.Status = (int)PaymentStatus.Fail;
                payOrder.ErrMsg = bizContent.ReturnMsg;
            }
            await dbContext.SaveChangesAsync();

            // 返回应答报文
            B2cNotifyResponse rsp = new B2cNotifyResponse
            {
                ResponseBizContent = new B2cNotifyResponse.BizContent
                {
                    ReturnCode = 0,
                    ReturnMsg = "success",
                    MsgId = bizContent.MsgId
                },
                SignType = req.Sign_type
            };

            string response = JsonSerializer.Serialize(rsp, serializerOptions);
            RsaUtil rsaUtil = new RsaUtil(privateKey, null);
            rsp.Sign = rsaUtil.Sign(response);

            return JsonSerializer.Serialize(rsp, serializerOptions); ;
        }
    }
}