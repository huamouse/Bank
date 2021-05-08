using Bank.Domains.Payment;
using Bank.Domains.Payment.Entities;
using Bank.Domains.Payment.Services;
using CPTech.Core;
using CPTech.Extension;
using CPTech.Models;
using CPTech.Security;
using Icbc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bank.Controllers
{
    [ApiController]
    [Route("pay")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> logger;
        private readonly IPaymentRepository paymentRepository;
        private readonly IDistributedCache cache;
        private readonly IEnumerable<IPaymentService> paymentServices;

        private readonly HttpClient httpClient;

        public PaymentController(ILogger<PaymentController> logger,
            IHttpClientFactory httpClientFactory,
            IPaymentRepository paymentRepository,
            IDistributedCache cache,
            IEnumerable<IPaymentService> paymentServices)
        {
            this.logger = logger;

            httpClient = httpClientFactory.CreateClient();

            this.paymentRepository = paymentRepository;
            this.paymentServices = paymentServices;

            try
            {
                cache.GetString("pay");
                this.cache = cache;
            }
            catch (Exception)
            {
                logger.LogError("Redis connect failed!");
            }
        }

        [HttpGet]
        public ResultModel CommTest()
        {
            return ResultModel.Ok("Comm test success!");
        }

        [Authorize]
        [HttpPost("orderCreate")]
        public async Task<ResultModel> OrderCreate(OrderCreateDto req)
        {
            if (req.Amount <= 0) throw new NetException(500, $"支付金额非法：{req.Amount}");

            logger.LogInformation($"create order req: {JsonSerializer.Serialize(req, Constants.JsonSerializerOption)}");
            string token = HmacSha256.Compute(JsonSerializer.Serialize(req, Constants.JsonSerializerOption), this.GetType().Name);

            var cacheToken = cache?.GetString(token);
            if (cacheToken is { Length: > 0 })
                throw new NetException(500, $"相同请求3s内禁止重复提交：{cacheToken}");
            else
                cache?.SetString(token, JsonSerializer.Serialize(req, Constants.JsonSerializerOption),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5) });

            if (req.OldOrderId != null)
            {
                PayOrder payOrderCancel = await paymentRepository.OrderQueryAsync(req.OldOrderId.Value);

                if (payOrderCancel != null)
                {
                    var paymentService = GetPaymentService(payOrderCancel.Channel);
                    var rspClose = paymentService.OrderClose(new OrderQueryReq
                    {
                        OrderNo = payOrderCancel.Id.ToString(),
                        PayType = payOrderCancel.PayType,
                        Payee = payOrderCancel.Payee
                    });
                    if (rspClose is { Status: PaymentStatus.Closed })
                    {
                        payOrderCancel.Status = (int)PaymentStatus.Closed;
                        payOrderCancel.CloseId = req.CreatorId;
                        payOrderCancel.CloseTime = DateTime.Now;
                        await paymentRepository.OrderCloseAsync(payOrderCancel);
                    }
                }
            }

            PayOrder payOrder = new()
            {
                Tag = req.Tag,
                Payee = req.Payee,
                Amount = decimal.ToInt32(req.Amount * 100),
                Note = req.Note,
                CreatorId = req.CreatorId
            };
            //payOrder.OrderNo = payOrder.Id;

            await paymentRepository.OrderCreateAsync(payOrder);

            return ResultModel.Ok(new { OrderId = payOrder.Id });
        }

        [Authorize]
        [HttpPost("OrderPay")]
        public async Task<string> OrderPay(OrderPayDto req)
        {
            logger.LogInformation($"order pay req: {JsonSerializer.Serialize(req, Constants.JsonSerializerOption)}");

            var result = cache?.GetString(req.OrderId.ToString());
            if (result is { Length: > 0 }) return result;

            var payOrder = await paymentRepository.OrderQueryAsync(req.OrderId) ?? throw new NetException(500, "订单已关闭或无效的订单号");
            if (payOrder.Status == (int)PaymentStatus.Success) throw new NetException(500, "已支付订单禁止重复支付！");

            payOrder.Payer = req.Payer;
            payOrder.PayerName = req.PayerName;
            payOrder.PayType = Enum.Parse<PayTypeEnum>(req.PayType, true);
            payOrder.Channel = Enum.Parse<ChannelEnum>(req.Channel, true);
            var paymentService = GetPaymentService(payOrder.Channel);

            OrderPayReq orderPayReq = new()
            {
                OrderNo = payOrder.Id.ToString(),
                PayType = payOrder.PayType,
                Amount = payOrder.Amount,
                Note = payOrder.Note,
                Payer = payOrder.Payer,
                PayerName = payOrder.PayerName,
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
            orderPayLog.ReturnCode = rsp.Code.ToString();
            orderPayLog.ReturnMsg = rsp.Message;
            orderPayLog.Response = rsp.Data;
            await paymentRepository.UpdateAsync(orderPayLog);

            payOrder.PayTime = DateTime.Now;
            await paymentRepository.UpdateAsync(payOrder);

            ResultModel resultModel = new()
            {
                Code = rsp.Code == 0 ? 200 : rsp.Code,
                Message = rsp.Code == 0 ? rsp.FlowNo : rsp.Message,
                Data = rsp.Data
            };
            result = JsonSerializer.Serialize(resultModel, Constants.JsonSerializerOption);
            Console.WriteLine($"order pay response: {JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption)}");
            Console.WriteLine($"order pay response2: {result}");

            cache?.SetString(req.OrderId.ToString(), result,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20) });

            return result;
        }

        [Authorize]
        [HttpGet("OrderQuery/{orderId}")]
        public async Task<ResultModel> OrderQuery(long orderId)
        {
            var payOrder = await paymentRepository.FindAsync<PayOrder>(orderId) ?? throw new NetException(500, "无效的订单号");
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

            var paymentService = GetPaymentService(payOrder.Channel);
            var rsp = paymentService.OrderQuery(new OrderQueryReq
            {
                OrderNo = payOrder.Id.ToString(),
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
                Code = rsp.Code == 0 ? 200 : rsp.Code,
                Message = rsp.Message,
                Data = new
                {
                    orderId,
                    rsp.Amount,
                    rsp.Status,
                    rsp.FlowNo,
                    PayTime = rsp.EndTime?.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };
        }

        [Authorize]
        [HttpPost("orderCancel")]
        public async Task<ResultModel> OrderCancel((long orderId, long? CancelerId) req)
        {
            var payOrder = await paymentRepository.OrderQueryAsync(req.orderId) ?? throw new NetException(500, "订单已关闭或无效的订单号");
            if (payOrder.Channel == ChannelEnum.Unknown)
            {
                payOrder.Status = (int)PaymentStatus.Closed;
                payOrder.CloseId = req.CancelerId;
                payOrder.CloseTime = DateTime.Now;
                await paymentRepository.SaveChangesAsync();

                return ResultModel.Ok($"订单:{payOrder.Id}已手动关闭！");
            }

            var paymentService = this.GetPaymentService(payOrder.Channel);
            var rspClose = paymentService?.OrderClose(new OrderQueryReq
            {
                OrderNo = payOrder.Id.ToString(),
                PayType = payOrder.PayType,
                Payee = payOrder.Payee
            });

            if (rspClose?.Status == PaymentStatus.Closed)
            {
                payOrder.Status = (int)PaymentStatus.Canceled;
                payOrder.CloseId = req.CancelerId;
                payOrder.CloseTime = DateTime.Now;
                await paymentRepository.OrderCloseAsync(payOrder);

                return ResultModel.Ok($"订单:{req.orderId}已成功关闭！");
            }

            return ResultModel.Error(500, $"订单:{req.orderId}关闭失败！");
        }


        [HttpGet("memberApply/{payeeNo}")]
        public ResultModel MemberApply(string payeeNo)
        {
            var icbcService = (IcbcService)GetPaymentService(ChannelEnum.ICBC);
            var rspClose = icbcService?.MemberApply(payeeNo);

            return ResultModel.Ok(rspClose);
        }

        [HttpPost("eventreceive")]
        public async Task<string> EventReceive([FromForm] B2cNotifyDto req)
        {
            Console.WriteLine("callback:" + HttpContext.Request.ContentType);
            Console.WriteLine($"callback req: {JsonSerializer.Serialize(req, Constants.JsonSerializerOption)}");

            var paymentService = GetPaymentService(ChannelEnum.ICBC);

            var bizContent = JsonSerializer.Deserialize<B2cNotifyDto.BizContent>(req.Biz_content, Constants.JsonSerializerOption);
            long orderNo = long.Parse(bizContent.OutTradeNo);
            var payOrder = await paymentRepository.OrderQueryAsync(orderNo) ?? throw new NetException(500, "无效的订单号");

            // 更新数据库
            if (bizContent.ReturnCode == "0" && payOrder.Status != (int)PaymentStatus.Success)
            {
                payOrder.Status = (int)PaymentStatus.Success;
                payOrder.EndTime = DateTime.ParseExact(bizContent.PayTime, "yyyyMMddHHmmss", null);
                payOrder.FlowNo = bizContent.OrderId;
                payOrder.OrderNo = bizContent.ThirdTradeNo;
                payOrder.ErrMsg = bizContent.ReturnMsg;

                await paymentRepository.UpdateAsync(payOrder);
            }
            else if (bizContent.ReturnCode == "1" && payOrder.Status != (int)PaymentStatus.Fail)
            {
                payOrder.Status = (int)PaymentStatus.Fail;
                payOrder.ErrMsg = bizContent.ReturnMsg;

                await paymentRepository.UpdateAsync(payOrder);
            }

            // 通知业务系统
            var payNotify = await paymentRepository.SelectNotifyAsync(payOrder.Tag);
            if (payNotify?.Url is { Length: > 0 } && payOrder is { Status: (int)PaymentStatus.Success })
            {
                var notifyResult = await httpClient.PostJsonAsync<ResultModel, ResultModel>(payNotify.Url, ResultModel.Ok(new
                {
                    OrderId = payOrder.Id,
                    Amount = decimal.Divide(payOrder.Amount, 100),
                    payOrder.Status,
                    payOrder.FlowNo,
                    PayTime = payOrder.EndTime?.ToString("yyyy-MM-dd HH:mm:ss")
                }));
                if (notifyResult is not { Code: 200 })
                {
                    logger.LogError($"OrderId:{payOrder.Id}推送失败！");
                    return "error";
                }
            }

            // 返回应答报文
            return paymentService.OrderNotify(new OrderNotifyReq
            {
                PayType = PayTypeEnum.WechatPay,
                Channel = ChannelEnum.ICBC,
                MsgId = bizContent.MsgId
            });
        }

        [HttpPost("b2bnotify")]
        public async Task<string> B2BNotify([FromForm] B2bNotifyDto req)
        {
            Console.WriteLine($"b2bCallback req: {JsonSerializer.Serialize(req, Constants.JsonSerializerOption)}");

            var bizContent = JsonSerializer.Deserialize<B2bNotifyDto.BizContent>(req.Biz_content, Constants.JsonSerializerOption);
            long orderNo = long.Parse(bizContent.PartnerSeq);
            var payOrder = await paymentRepository.OrderQueryAsync(orderNo) ?? throw new NetException(500, "无效的订单号");

            // 更新数据库
            if (bizContent.PayStatus == "0")
            {
                payOrder.Status = (int)(bizContent.PayStatus switch
                {
                    "1" or "8" => PaymentStatus.Success,
                    "2" => PaymentStatus.Fail,
                    _ => PaymentStatus.Paying
                });
                payOrder.FlowNo = bizContent.SerialNo;
                payOrder.ErrMsg = bizContent.ReturnMsg;

                await paymentRepository.UpdateAsync(payOrder);
            }
            else if (bizContent.ReturnCode == "1")
            {
                payOrder.Status = (int)PaymentStatus.Fail;
                payOrder.ErrMsg = bizContent.ReturnMsg;

                await paymentRepository.UpdateAsync(payOrder);
            }

            // 通知业务系统
            var payNotify = await paymentRepository.SelectNotifyAsync(payOrder.Tag);
            if (payNotify?.Url is { Length: > 0 } && payOrder is { Status: (int)PaymentStatus.Success })
            {
                var notifyResult = await httpClient.PostJsonAsync<ResultModel, ResultModel>(payNotify.Url, ResultModel.Ok(new
                {
                    OrderId = payOrder.Id,
                    Amount = decimal.Divide(payOrder.Amount, 100),
                    payOrder.Status,
                    payOrder.FlowNo
                }));
                if (notifyResult is not { Code: 200 })
                {
                    logger.LogError($"OrderId:{payOrder.Id}推送失败！");
                    return "error";
                }
            }

            // 返回应答报文
            return "ok";
        }

        #region 获取支付服务

        private IPaymentService GetPaymentService(ChannelEnum channel)
            => paymentServices.FirstOrDefault(service => service.GetType().Name.Equals(channel + "Service",
                StringComparison.OrdinalIgnoreCase)) ?? throw new NetException(500, $"未实现的通道服务:{channel}Service");

        #endregion
    }
}
