using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Bank.Domains.Payment;
using Bank.Domains.Payment.Entities;
using Bank.Domains.Payment.Services;
using Cptech.Extension;
using CPTech.Core;
using CPTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bank.Controllers
{
    [ApiController]
    [Route("pay")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> logger;
        private readonly IPaymentRepository paymentRepository;
        private readonly IEnumerable<IPaymentService> paymentServices;

        private readonly HttpClient httpClient;

        public PaymentController(ILogger<PaymentController> logger,
            IHttpClientFactory httpClientFactory,
            IPaymentRepository paymentRepository,
            IEnumerable<IPaymentService> paymentServices)
        {
            this.logger = logger;

            httpClient = httpClientFactory.CreateClient();

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
            logger.LogInformation($"create order req: {JsonSerializer.Serialize(req, Constants.JsonSerializerOption)}");

            PayOrder payOrder = new()
            {
                OrderNo = SnowFlake.NextId(),
                Payee = req.Payee,
                Amount = decimal.ToInt32(req.Amount * 100),
                Note = req.Note,
                CreatorId = req.CreatorId
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
            var payOrder = await paymentRepository.FindAsync<PayOrder>(req.OrderId) ?? throw new NetException(500, "无效的订单号");
            if (payOrder.Status == (int)PaymentStatus.Success) throw new NetException(500, "已支付订单禁止重复支付！");

            payOrder.PayType = Enum.Parse<PayTypeEnum>(req.PayType, true);
            payOrder.Channel = Enum.Parse<ChannelEnum>(req.Channel, true);
            var paymentService = this.GetPaymentService(payOrder.Channel);

            // 取消旧订单
            if (payOrder.PayTime != null)
            {
                OrderQueryReq orderQueryReq = new()
                {
                    OrderNo = payOrder.OrderNo.ToString(),
                    PayType = payOrder.PayType,
                    Payee = payOrder.Payee
                };

                PayOrderLog closeOrderLog = new()
                {
                    PayType = payOrder.PayType,
                    Channel = payOrder.Channel,
                    Request = "OrderClose:" + JsonSerializer.Serialize(orderQueryReq, Constants.JsonSerializerOption)
                };
                await paymentRepository.AddPayOrderLogAsync(closeOrderLog);

                var rspClose = paymentService.OrderClose(orderQueryReq);

                closeOrderLog.Response = rspClose;
                await paymentRepository.UpdateAsync(closeOrderLog);

                payOrder.OrderNo = SnowFlake.NextId();
            }
            else
                payOrder.PayTime = DateTime.Now;
            payOrder.Payer = req.Payer;

            await paymentRepository.UpdateAsync(payOrder);

            OrderPayReq orderPayReq = new()
            {
                OrderNo = payOrder.OrderNo.ToString(),
                PayType = payOrder.PayType,
                Amount = payOrder.Amount,
                Note = payOrder.Note,
                //NotifyUrl = "https://www.cptech.com.cn/pay/eventreceive",
                Payer = payOrder.Payer,
                Payee = payOrder.Payee
            };
            PayOrderLog orderPayLog = new()
            {
                PayType = payOrder.PayType,
                Channel = payOrder.Channel,
                Request = "OrderPay:" + JsonSerializer.Serialize(orderPayReq, Constants.JsonSerializerOption)
            };
            await paymentRepository.AddPayOrderLogAsync(orderPayLog);

            BasePayRsp rsp = paymentService.OrderPay(orderPayReq);
            orderPayLog.Response = rsp.Data;
            await paymentRepository.UpdateAsync(orderPayLog);

            return new ResultModel
            {
                Code = rsp.Code,
                Message = rsp.Message,
                Data = rsp.Data
            };
        }

        [HttpGet("OrderQuery/{orderId}")]
        public async Task<ResultModel> OrderQuery(long orderId)
        {
            var payOrder = await paymentRepository.FindAsync<PayOrder>(orderId);
            if (payOrder.Status == (int)PaymentStatus.Success)
            {
                return ResultModel.Ok(new
                {
                    Amount = decimal.Divide(payOrder.Amount, 100),
                    payOrder.Status,
                    payOrder.FlowNo,
                    PayTime = payOrder.EndTime?.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            var paymentService = this.GetPaymentService(payOrder.Channel);
            var rsp = paymentService.OrderQuery(new OrderQueryReq
            {
                OrderNo = payOrder.OrderNo.ToString(),
                PayType = payOrder.PayType,
                Payee = payOrder.Payee
            });

            if (rsp?.Status != null && payOrder.Status != (int)rsp.Status) payOrder.Status = (int)rsp.Status;
            if (rsp?.EndTime != null && payOrder.EndTime != rsp.EndTime) payOrder.EndTime = rsp.EndTime;
            if (rsp?.FlowNo != null && payOrder.FlowNo != rsp.FlowNo) payOrder.FlowNo = rsp.FlowNo;
            if (rsp?.Reserve != null && payOrder.Reserve != rsp.Reserve) payOrder.Reserve = rsp.Reserve;
            payOrder.ErrMsg = rsp.Message;
            await paymentRepository.UpdateAsync(payOrder);

            return new ResultModel
            {
                Code = rsp.Code,
                Message = rsp.Message,
                Data = new
                {
                    rsp.Amount,
                    rsp.Status,
                    rsp.FlowNo,
                    PayTime = rsp.EndTime?.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };
        }

        [HttpPost("orderClose")]
        public async Task<ResultModel> OrderClose((long OrderNo, long? ClosersId) req)
        {
            var payOrder = await paymentRepository.OrderQueryLastAsync(req.OrderNo) ?? throw new NetException(500, "无效的订单号");

            if (payOrder.Channel != ChannelEnum.Unknown)
            {
                var paymentService = this.GetPaymentService(payOrder.Channel);

                var rspClose = paymentService?.OrderClose(new OrderQueryReq
                {
                    OrderNo = payOrder.OrderNo.ToString(),
                    PayType = payOrder.PayType,
                    Payee = payOrder.Payee
                });
            }

            payOrder.CloseId = req.ClosersId;
            payOrder.CloseTime = DateTime.Now;
            await paymentRepository.OrderCloseAsync(payOrder);

            return ResultModel.Ok(payOrder.Status == (int)PaymentStatus.Closed ? $"订单:{req.OrderNo}已成功关闭！" : $"订单:{req.OrderNo}关闭失败！");
        }

        [HttpPost("eventreceive")]
        public async Task<string> EventReceive([FromForm] B2cNotifyDto req)
        {
            Console.WriteLine("callback:" + HttpContext.Request.ContentType);
            Console.WriteLine($"callback req: {JsonSerializer.Serialize(req, Constants.JsonSerializerOption)}");

            var paymentService = this.GetPaymentService(ChannelEnum.ICBC);

            var bizContent = JsonSerializer.Deserialize<B2cNotifyDto.BizContent>(req.Biz_content, Constants.JsonSerializerOption);
            long orderNo = long.Parse(bizContent.OutTradeNo);
            var payOrder = await paymentRepository.OrderQueryLastAsync(orderNo);

            // 更新数据库
            if (bizContent.ReturnCode == "0" && payOrder.Status != (int)PaymentStatus.Success)
            {
                payOrder.Status = (int)PaymentStatus.Success;
                payOrder.EndTime = DateTime.ParseExact(bizContent.PayTime, "yyyyMMddHHmmss", null);
                payOrder.FlowNo = bizContent.ThirdTradeNo;
                payOrder.Reserve = bizContent.order_id;
            }
            else if (bizContent.ReturnCode == "1" && payOrder.Status != (int)PaymentStatus.Fail)
                payOrder.Status = (int)PaymentStatus.Fail;

            payOrder.ErrMsg = bizContent.ReturnMsg;
            await paymentRepository.UpdateAsync(payOrder);

            // 通知业务系统
            var payNotify = await paymentRepository.SelectNotifyAsync(payOrder.Tag);
            if (payNotify?.Url is { Length: > 0 })
            {
                await httpClient.PostJsonAsync<ResultModel, ResultModel>(payNotify.Url, ResultModel.Ok(new
                {
                    OrderId = payOrder.Id,
                    Amount = decimal.Divide(payOrder.Amount, 100),
                    payOrder.Status,
                    payOrder.FlowNo,
                    PayTime = payOrder.EndTime?.ToString("yyyy-MM-dd HH:mm:ss")
                }));
            }

            // 返回应答报文
            return paymentService.OrderNotify(new OrderNotifyReq
            {
                PayType = PayTypeEnum.WechatPay,
                Channel = ChannelEnum.ICBC,
                MsgId = bizContent.MsgId
            });
        }

        #region 获取支付服务

        private IPaymentService GetPaymentService(ChannelEnum channel)
            => paymentServices.FirstOrDefault(service => service.GetType().Name.Equals(channel + "Service",
                StringComparison.OrdinalIgnoreCase)) ?? throw new NetException(500, $"未实现的通道服务:{channel}Service");

        #endregion
    }
}
