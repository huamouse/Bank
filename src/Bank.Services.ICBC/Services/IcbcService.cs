using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Bank.Domains.Payment;
using Bank.Domains.Payment.Services;
using CPTech.Core;
using CPTech.Security;
using Icbc.Business;
using Icbc.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Icbc.Services
{
    public class IcbcService : IPaymentService
    {
        private readonly IcbcOptions options;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<IcbcService> logger;

        public IcbcService(IHttpClientFactory clientFactory,
            IOptions<IcbcOptions> icbcOptions,
            ILogger<IcbcService> logger)
        {
            this.clientFactory = clientFactory;
            this.logger = logger;

            options = icbcOptions.Value;
        }

        public BasePayRsp OrderPay(OrderPayReq payment)
        {
            BasePayRsp result = null;
            switch (payment.PayType)
            {
                case PayTypeEnum.Unknown:
                    throw new NotImplementedException();
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cPay(payment);
                    break;
                case PayTypeEnum.Bank:
                    result = B2bPay(payment);
                    break;
            }

            return result;
        }

        public OrderQueryRsp OrderQuery(OrderQueryReq payQuery)
        {
            OrderQueryRsp result = null;
            switch (payQuery.PayType)
            {
                case PayTypeEnum.Unknown:
                    throw new NotImplementedException();
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cQuery(payQuery);
                    break;
                case PayTypeEnum.Bank:
                    result = B2bQuery(payQuery);
                    break;
            }

            return result;
        }

        public string OrderClose(OrderQueryReq payQuery)
        {
            string result = null;
            switch (payQuery.PayType)
            {
                case PayTypeEnum.Unknown:
                    throw new NotImplementedException();
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cClose(payQuery);
                    break;
                case PayTypeEnum.Bank:
                    result = B2bClose(payQuery);
                    break;
            }

            return result;
        }

        public string OrderNotify(OrderNotifyReq payNotify)
        {
            string result = null;
            switch (payNotify.PayType)
            {
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cNotify(payNotify);
                    break;
                case PayTypeEnum.Bank:
                    result = B2bNotify(payNotify);
                    break;
            }

            return result;
        }

        #region 聚合支付
        private BasePayRsp B2cPay(OrderPayReq orderPayReq)
        {
            BasePayRsp rsp = new BasePayRsp();

            IcbcClient client = new IcbcClient(options.AppId, options.PrivateKey, options.GatewayPublicKey, clientFactory.CreateClient());
            var merInfo = options.MerInfos.FirstOrDefault(m => m.AccNo == orderPayReq.Payee) ?? throw new Exception("无效的Merinfo信息!");

            var bizContent = new CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1.CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1Biz()
            {
                MerId = merInfo.Id,
                OutTradeNo = orderPayReq.OrderNo,
                PayMode = orderPayReq.PayType == PayTypeEnum.WechatPay ? "9" : "",
                AccessType = orderPayReq.PayType == PayTypeEnum.WechatPay ? "9" : "",
                MerPrtclNo = merInfo.PrtclNo,
                OrigDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                DeciveInfo = "0123456789",  //设备号
                Body = orderPayReq.Note,
                FeeType = "001",
                SpbillCreateIp = "192.168.0.1",
                TotalFee = orderPayReq.Amount.ToString(),
                MerUrl = options.NotifyUrl,
                ShopAppid = options.WeixinAppId,
                IcbcAppId = options.AppId,
                OpenId = orderPayReq.Payer,
                NotifyType = "HS",
                ResultType = "0"
            };

            var response = (CardbusinessAggregatepayB2cOnlineConsumepurchaseResponseV1)client.Execute(
                new CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1()
                {
                    Content = bizContent
                });

            int returnCode = int.Parse(response.ReturnCode.ToString());
            return new BasePayRsp
            {
                Code = returnCode == 0 ? 200 : returnCode,
                Message = response.ReturnMsg,
                Data = response.WxDataPackage
            };
        }

        private OrderQueryRsp B2cQuery(OrderQueryReq orderQueryReq)
        {
            IcbcClient client = new IcbcClient(options.AppId, options.PrivateKey, options.GatewayPublicKey, clientFactory.CreateClient());
            var merInfo = options.MerInfos.FirstOrDefault(m => m.AccNo == orderQueryReq.Payee) ?? throw new Exception("无效的Merinfo信息!");

            var bizContent = new CardbusinessAggregatepayB2cOnlineOrderqryRequestV1.CardbusinessAggregatepayB2cOnlineOrderqryRequestV1Biz()
            {
                MerId = merInfo.Id,
                OutTradeNo = orderQueryReq.OrderNo,
                DealFlag = orderQueryReq.QueryType == QueryTypeEnum.Close ? "1" : "0",
                IcbcAppId = options.AppId
            };

            var rsp = (CardbusinessAggregatepayB2cOnlineOrderqryResponseV1)client.Execute(
                new CardbusinessAggregatepayB2cOnlineOrderqryRequestV1()
                {
                    Content = bizContent
                });

            int returnCode = int.Parse(rsp.ReturnCode.ToString());
            bool isSuccess = returnCode == 0;

            return new OrderQueryRsp
            {
                Code = isSuccess ? 200 : returnCode,
                Message = rsp.ReturnMsg,
                Status = isSuccess ? PaymentStatus.Success : default(PaymentStatus?),
                EndTime = isSuccess ? DateTime.ParseExact(rsp.PayTime, "yyyyMMddHHmmss", null) : default(DateTime?),
                Amount = isSuccess ? decimal.Parse(rsp.TotalAmt) / 100 : default(decimal?),
                FlowNo = isSuccess ? rsp.ThirdTradeNo : null,   // 交易单号
                Reserve = isSuccess ? rsp.OrderId : null,   // 商户单号
                Data = JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption)
            };
        }

        private string B2cClose(OrderQueryReq orderQueryReq)
        {
            IcbcClient client = new IcbcClient(options.AppId, options.PrivateKey, options.GatewayPublicKey, clientFactory.CreateClient());
            var merInfo = options.MerInfos.FirstOrDefault(m => m.AccNo == orderQueryReq.Payee) ?? throw new Exception("无效的Merinfo信息!");

            var bizContent = new CardbusinessAggregatepayB2cOnlineOrderqryRequestV1.CardbusinessAggregatepayB2cOnlineOrderqryRequestV1Biz()
            {
                MerId = merInfo.Id,
                OutTradeNo = orderQueryReq.OrderNo,
                DealFlag = "1",
                IcbcAppId = options.AppId
            };

            var response = (CardbusinessAggregatepayB2cOnlineOrderqryResponseV1)client.Execute(
                new CardbusinessAggregatepayB2cOnlineOrderqryRequestV1()
                {
                    Content = bizContent
                });

            return JsonSerializer.Serialize(response, Constants.JsonSerializerOption);
        }

        private string B2cNotify(OrderNotifyReq payNotify)
        {
            B2cNotifyResponse rsp = new B2cNotifyResponse
            {
                ResponseBizContent = new B2cNotifyResponse.BizContent
                {
                    ReturnCode = 0,
                    ReturnMsg = "success",
                    MsgId = payNotify.MsgId
                },
                SignType = options.PrivateKeyType
            };

            string response = JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption);
            RsaUtil rsaUtil = new RsaUtil(options.PrivateKey, null);
            rsp.Sign = rsaUtil.Sign(response);

            return JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption);
        }
        #endregion

        #region E企付
        private BasePayRsp B2bPay(OrderPayReq orderPayReq)
        {
            IcbcClient client = new IcbcClient(options.BizAppId, options.BizPrivateKey, options.BizGatewayPublicKey, clientFactory.CreateClient());
            var bizMerInfo = options.BizMerInfos[0];

            var recvMallInfoList = new List<MybankPayCpayCppayapplyRequestV2.RecvMallInfo>
            {
                new MybankPayCpayCppayapplyRequestV2.RecvMallInfo
                {
                    MallCode = bizMerInfo.Id,
                    MallName = bizMerInfo.AccName,
                    PayeeCompanyName = bizMerInfo.AccName,
                    PayeeSysflag = "1",
                    PayeeAccno = bizMerInfo.AccNo,
                    PayAmount = orderPayReq.Amount.ToString()
                }
            };

            var goodsInfoList = new List<MybankPayCpayCppayapplyRequestV2.GoodsInfo>
            {
                new MybankPayCpayCppayapplyRequestV2.GoodsInfo
                {
                    GoodsSubId = "1",
                    GoodsName = orderPayReq.Note,
                    PayeeCompanyName = bizMerInfo.AccName,
                    GoodsNumber = "1",
                    GoodsAmt = orderPayReq.Amount.ToString(),
                    GoodsUnit = ""
                }
            };

            var bizContent = new MybankPayCpayCppayapplyRequestV2.MybankPayCpayCppayapplyRequestV2Biz
            {
                AgreeCode = bizMerInfo.PrtclNo,
                PartnerSeq = orderPayReq.OrderNo,
                PayChannel = "1",
                InternationalFlag = "1",
                PayMode = "1",
                PayEntitys = "1",
                AsynFlag = "0",
                PayMemno = orderPayReq.Payer,
                Orgcode = "",
                OrderCode = orderPayReq.OrderNo,
                OrderAmount = orderPayReq.Amount.ToString(),
                OrderCurr = "001",
                SumPayamt = orderPayReq.Amount.ToString(),
                OrderRemark = orderPayReq.Note,
                RceiptRemark = orderPayReq.Note,
                SubmitTime = "20201130" + DateTime.Now.ToString("HHmmss"),
                ReturnUrl = options.ReturnUrl,
                CallbackUrl = options.NotifyUrl,
                IdentityMode = "0",
                PayeeList = recvMallInfoList,
                GoodsList = goodsInfoList
            };

            var response = (MybankPayCpayCppayapplyResponseV2)client.Execute(new MybankPayCpayCppayapplyRequestV2
            {
                Content = bizContent
            });

            int returnCode = int.Parse(response.ReturnCode.ToString());
            return new BasePayRsp
            {
                Code = returnCode == 0 ? 200 : returnCode,
                Message = response.ReturnMsg,
                Data = JsonSerializer.Serialize(response, Constants.JsonSerializerOption)
            };
        }

        private OrderQueryRsp B2bQuery(OrderQueryReq orderQueryReq)
        {
            IcbcClient client = new IcbcClient(options.BizAppId, options.BizPrivateKey, options.BizGatewayPublicKey, clientFactory.CreateClient());
            var bizMerInfo = options.BizMerInfos[0];

            var bizContent = new MybankPayCpayCporderqueryRequestV2.QueryPayApplyRequestV2Biz
            {
                AgreeCode = bizMerInfo.PrtclNo,
                PartnerSeq = orderQueryReq.OrderNo,
                OrderCode = orderQueryReq.OrderNo
            };

            var response = (MybankPayCpayCporderqueryResponseV2)client.Execute(
                new MybankPayCpayCporderqueryRequestV2
                {
                    Content = bizContent
                });

            int returnCode = int.Parse(response.ReturnCode.ToString());
            bool isSuccess = returnCode == 0;

            return new OrderQueryRsp
            {
                Code = isSuccess ? 200 : returnCode,
                Message = response.ReturnMsg,
                Status = isSuccess ? PaymentStatus.Success : PaymentStatus.Paying,
                Data = JsonSerializer.Serialize(returnCode, Constants.JsonSerializerOption)
            };
        }

        private string B2bClose(OrderQueryReq orderQueryReq)
        {
            IcbcClient client = new IcbcClient(options.BizAppId, options.BizPrivateKey, options.BizGatewayPublicKey, clientFactory.CreateClient());
            var bizMerInfo = options.BizMerInfos[0];

            var bizContent = new MybankPayCpayCpordercloseRequestV1.MybankPayCpayCpordercloseV1RequestV1Biz
            {
                AgreeCode = bizMerInfo.PrtclNo,
                PartnerSeq = orderQueryReq.OrderNo,
                OrderCode = orderQueryReq.OrderNo
            };

            var response = (MybankPayCpayCpordercloseResponseV1)client.Execute(
                new MybankPayCpayCpordercloseRequestV1
                {
                    Content = bizContent
                });

            int returnCode = int.Parse(response.ReturnCode.ToString());
            bool isSuccess = returnCode == 0;

            return JsonSerializer.Serialize(response, Constants.JsonSerializerOption);
        }

        private string B2bNotify(OrderNotifyReq payNotify)
        {
            throw new NullReferenceException();
        }
        #endregion
    }
}
