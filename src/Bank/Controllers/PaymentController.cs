using Bank.Domains.Payment;
using CPTech.Core;
using CPTech.Models;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IPaymentRepository paymentRepository;
        private readonly IEnumerable<IPaymentService> paymentServices;

        public PaymentController(ILogger<PaymentController> logger,
            IPaymentRepository paymentRepository,
            IEnumerable<IPaymentService> paymentServices)
        {
            this.logger = logger;
            this.paymentRepository = paymentRepository;
            this.paymentServices = paymentServices;
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
                OrderNo = SnowFlake.NextId(),
                Amount = decimal.ToInt32(req.Amount * 100),
                Note = req.Note,
                CreatorId = req.CreateId
            };

            await paymentRepository.OrderCreateAsync(payOrder);

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

            var payOrder = await paymentRepository.FindAsync<PayOrder>(req.OrderId) ?? throw new NetException(500, "无效的订单号");

            // 取消旧订单
            if (payOrder.PayTime > new DateTime(2000, 1, 1))
            {
                //var rspClose = paymentService.Query(new PayQuery
                //{
                //    OrderNo = payOrder.OrderNo.ToString(),
                //    PayType = payOrder.PayType,
                //    QueryType = QueryTypeEnum.Close
                //});

                //payOrder.OrderNo = SnowFlake.NextId();
            }
            else
                payOrder.PayTime = DateTime.Now;

            payOrder.PayType = Enum.Parse<PayTypeEnum>(req.PayType, true);
            payOrder.Channel = Enum.Parse<ChannelEnum>(req.Channel, true);
            payOrder.Payer = req.Payer;

            await paymentRepository.UpdateAsync(payOrder);

            string response = paymentService.OrderPay(new Payment()
            {
                OrderNo = payOrder.OrderNo.ToString(),
                PayType = payOrder.PayType,
                Amount = payOrder.Amount,
                Note = payOrder.Note,
                NotifyUrl = "https://www.cptech.com.cn/pay/eventreceive",
                Payer = payOrder.Payer
            });

            return ResultModel.Ok(response);
        }

        [HttpGet("OrderQuery/{orderId}")]
        public async Task<ResultModel> OrderQuery(long orderId)
        {
            var payOrder = await paymentRepository.FindAsync<PayOrder>(orderId);

            var paymentService = paymentServices.FirstOrDefault(service => service.GetType().Name.Equals(payOrder.Channel + "Service",
                StringComparison.OrdinalIgnoreCase)) ?? throw new NetException(500, $"未实现的通道服务:{payOrder.Channel}Service");

            string response = paymentService.OrderQuery(new PayQuery
            {
                OrderNo = payOrder.OrderNo.ToString(),
                PayType = payOrder.PayType
            });
            return ResultModel.Ok(response);
        }

        [HttpPost("orderClose")]
        public async Task<ResultModel> OrderClose((long OrderNo, string CloseId) req)
        {
            var payOrder = await paymentRepository.OrderQueryLastAsync(req.OrderNo) ?? throw new NetException(500, "无效的订单号");

            if (payOrder.Channel != ChannelEnum.Unknown)
            {
                var paymentService = paymentServices.FirstOrDefault(service => service.GetType().Name.Equals(payOrder.Channel + "Service",
                    StringComparison.OrdinalIgnoreCase));

                var rspClose = paymentService?.OrderClose(new PayQuery
                {
                    OrderNo = payOrder.OrderNo.ToString(),
                    PayType = payOrder.PayType
                });
            }

            payOrder.CloseId = req.CloseId;
            await paymentRepository.OrderCloseAsync(payOrder);

            return ResultModel.Ok(payOrder.Status == (int)PaymentStatus.Closed ? $"订单:{req.OrderNo}已成功关闭！" : $"订单:{req.OrderNo}关闭失败！");
        }

        [HttpPost("eventreceive")]
        public async Task<string> EventReceive([FromForm] B2cNotifyDto req)
        {
            Console.WriteLine("callback:" + HttpContext.Request.ContentType);
            Console.WriteLine($"callback req: {JsonSerializer.Serialize(req, Constants.SerializerOptions)}");
            var bizContent = JsonSerializer.Deserialize<B2cNotifyDto.BizContent>(req.Biz_content);

            var paymentService = paymentServices.FirstOrDefault(service => service.GetType().Name.Equals("IcbcService",
                StringComparison.OrdinalIgnoreCase)) ?? throw new NetException(500, $"未实现的通道服务:IcbcService");

            // 更新数据库
            long id = long.Parse(bizContent.OutTradeNo);
            var payOrder = await paymentRepository.FindAsync<PayOrder>(id);
            if (bizContent.ReturnCode == "0" && payOrder.Status != (int)PaymentStatus.Success)
                payOrder.Status = (int)PaymentStatus.Success;
            else if (bizContent.ReturnCode == "1" && payOrder.Status != (int)PaymentStatus.Fail)
            {
                payOrder.Status = (int)PaymentStatus.Fail;
                payOrder.ErrMsg = bizContent.ReturnMsg;
            }
            await paymentRepository.UpdateAsync(payOrder);

            // 返回应答报文
            return paymentService.Notify(new PayNotify
            {
                PayType = PayTypeEnum.WechatPay,
                Channel = ChannelEnum.ICBC,
                MsgId = bizContent.MsgId
            });
        }
    }
}