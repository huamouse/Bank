using Bank.Domains.Payment;
using Bank.Domains.Payment.Services;
using CPTech.Core;
using CPTech.Security;
using Icbc.Business;
using Icbc.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

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

        public BasePayRsp OrderPay(OrderPayReq orderPayReq)
        {
            BasePayRsp result = null;
            switch (orderPayReq.PayType)
            {
                case PayTypeEnum.Unknown:
                    throw new NotImplementedException();
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cPay(orderPayReq);
                    break;
                case PayTypeEnum.Bank:
                    result = B2bPay(orderPayReq);
                    break;
            }

            return result;
        }

        public OrderQueryRsp OrderQuery(OrderQueryReq orderQueryReq)
        {
            OrderQueryRsp result = null;
            switch (orderQueryReq.PayType)
            {
                case PayTypeEnum.Unknown:
                    throw new NotImplementedException();
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cQuery(orderQueryReq);
                    break;
                case PayTypeEnum.Bank:
                    result = B2bQuery(orderQueryReq);
                    break;
            }

            return result;
        }

        public BasePayRsp OrderClose(OrderQueryReq orderCloseReq)
        {
            BasePayRsp result = null;
            switch (orderCloseReq.PayType)
            {
                case PayTypeEnum.Unknown:
                    throw new NotImplementedException();
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cClose(orderCloseReq);
                    break;
                case PayTypeEnum.Bank:
                    result = B2bClose(orderCloseReq);
                    break;
            }

            return result;
        }

        public string OrderNotify(OrderNotifyReq orderNotifyReq)
        {
            string result = null;
            switch (orderNotifyReq.PayType)
            {
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cNotify(orderNotifyReq);
                    break;
                case PayTypeEnum.Bank:
                    result = B2bNotify(orderNotifyReq);
                    break;
            }

            return result;
        }

        public BasePayRsp OrderRefund(BasePayReq orderRefundReq)
        {
            throw new NullReferenceException();
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
                Attach = orderPayReq.Note,
                FeeType = "001",
                SpbillCreateIp = "192.168.0.1",
                TotalFee = orderPayReq.Amount.ToString(),
                MerUrl = options.NotifyUrl,
                ShopAppid = options.WeixinAppId,
                IcbcAppId = options.AppId,
                OpenId = orderPayReq.Payer,
                NotifyType = "HS",
                ResultType = "0",
                OrderApdInf = orderPayReq.Note
            };

            var response = (CardbusinessAggregatepayB2cOnlineConsumepurchaseResponseV1)client.Execute(
                new CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1() { Content = bizContent });

            return new BasePayRsp
            {
                Code = response.ReturnCode,
                Message = response.ReturnMsg,
                FlowNo = response.OrderId,
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
                DealFlag = "0",
                IcbcAppId = options.AppId
            };

            var rsp = (CardbusinessAggregatepayB2cOnlineOrderqryResponseV1)client.Execute(
                new CardbusinessAggregatepayB2cOnlineOrderqryRequestV1()
                {
                    Content = bizContent
                });
            Console.WriteLine($"query response: {JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption)}");

            var result = new OrderQueryRsp { Code = rsp.ReturnCode, Message = rsp.ReturnMsg };

            if (rsp.ReturnCode == 0)
            {
                result.Status = PaymentStatus.Success;
                result.EndTime = DateTime.ParseExact(rsp.PayTime, "yyyyMMddHHmmss", null);
                result.Amount = decimal.Parse(rsp.TotalAmt) / 100;
                result.FlowNo = rsp.OrderId;   // 工行单号
                result.Reserve = rsp.ThirdTradeNo;   // 商户单号
                result.Data = JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption);
            };

            return result;
        }

        private BasePayRsp B2cClose(OrderQueryReq orderCloseReq)
        {
            IcbcClient client = new IcbcClient(options.AppId, options.PrivateKey, options.GatewayPublicKey, clientFactory.CreateClient());
            var merInfo = options.MerInfos.FirstOrDefault(m => m.AccNo == orderCloseReq.Payee) ?? throw new Exception("无效的Merinfo信息!");

            var bizContent = new CardbusinessAggregatepayB2cOnlineOrderqryRequestV1.CardbusinessAggregatepayB2cOnlineOrderqryRequestV1Biz()
            {
                MerId = merInfo.Id,
                OutTradeNo = orderCloseReq.OrderNo,
                DealFlag = "1",
                IcbcAppId = options.AppId
            };

            var rsp = (CardbusinessAggregatepayB2cOnlineOrderqryResponseV1)client.Execute(
                new CardbusinessAggregatepayB2cOnlineOrderqryRequestV1() { Content = bizContent });

            var result = new BasePayRsp { Code = rsp.ReturnCode, Message = rsp.ReturnMsg };
            if (rsp.ReturnCode == 0) result.Status = PaymentStatus.Closed;

            return result;
        }

        private string B2cNotify(OrderNotifyReq payNotify)
        {
            var rsp = new B2cNotifyResponse
            {
                ResponseBizContent = new B2cNotifyResponse.BizContent
                {
                    ReturnCode = 0,
                    ReturnMsg = "success",
                    MsgId = payNotify.MsgId
                },
                SignType = options.PrivateKeyType
            };

            string signContent = JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption)[1..^1];
            RsaUtil rsaUtil = new RsaUtil(options.PrivateKey, null);
            rsp.Sign = rsaUtil.Sign(signContent);

            return JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption);
        }
        #endregion

        #region E企付
        private BasePayRsp B2bPay(OrderPayReq orderPayReq)
        {
            IcbcClient client = new IcbcClient(options.BizAppId, options.BizPrivateKey, options.BizGatewayPublicKey, clientFactory.CreateClient());
            var bizMerInfo = options.BizMerInfos.FirstOrDefault(e => e.AccNo == orderPayReq.Payee) ?? throw new Exception("无效的收款账户！");

            var recvMallInfoList = new List<MybankPayCpayCppayapplyRequestV2.RecvMallInfo>
            {
                new MybankPayCpayCppayapplyRequestV2.RecvMallInfo
                {
                    MallCode = bizMerInfo.Id,
                    MallName = bizMerInfo.AccName,
                    PayeeCompanyName = bizMerInfo.AccName,
                    PayeeSysflag = bizMerInfo.IcbcFlag,
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
                PayMode = bizMerInfo.PayMode,
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
                SubmitTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                //SubmitTime = "20210331" + DateTime.Now.ToString("HHmmss"),
                ReturnUrl = options.ReturnUrl,
                CallbackUrl = options.BizNotifyUrl,
                IdentityMode = "0",
                EnterpriseName = orderPayReq.PayerName,
                PayeeList = recvMallInfoList,
                GoodsList = goodsInfoList
            };

            var response = (MybankPayCpayCppayapplyResponseV2)client.Execute(
                new MybankPayCpayCppayapplyRequestV2 { Content = bizContent });

            return new BasePayRsp
            {
                Code = response.ReturnCode,
                Message = response.ReturnMsg,
                FlowNo = response.PartnerSeq,
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
                new MybankPayCpayCporderqueryRequestV2 { Content = bizContent });

            return new OrderQueryRsp
            {
                Code = response.ReturnCode,
                Message = response.ReturnMsg,
                Status = response.ReturnCode == 0 ? PaymentStatus.Success : PaymentStatus.Paying,
                FlowNo = response.PartnerSeq,
                Data = JsonSerializer.Serialize(response, Constants.JsonSerializerOption)
            };
        }

        private BasePayRsp B2bClose(OrderQueryReq orderCloseReq)
        {
            IcbcClient client = new IcbcClient(options.BizAppId, options.BizPrivateKey, options.BizGatewayPublicKey, clientFactory.CreateClient());
            var bizMerInfo = options.BizMerInfos[0];

            var bizContent = new MybankPayCpayCpordercloseRequestV1.MybankPayCpayCpordercloseV1RequestV1Biz
            {
                AgreeCode = bizMerInfo.PrtclNo,
                PartnerSeq = orderCloseReq.OrderNo,
                OrderCode = orderCloseReq.OrderNo
            };

            var rsp = (MybankPayCpayCpordercloseResponseV1)client.Execute(
                new MybankPayCpayCpordercloseRequestV1 { Content = bizContent });

            var result = new BasePayRsp { Code = rsp.ReturnCode, Message = rsp.ReturnMsg };
            if (rsp.ReturnCode == 0) result.Status = PaymentStatus.Closed;

            return result;
        }

        private string B2bNotify(OrderNotifyReq orderNotifyReq)
        {
            var rsp = new B2cNotifyResponse
            {
                ResponseBizContent = new B2cNotifyResponse.BizContent
                {
                    ReturnCode = 0,
                    ReturnMsg = "success",
                    MsgId = orderNotifyReq.MsgId
                },
                SignType = options.PrivateKeyType
            };

            string signContent = JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption)[1..^1];
            RsaUtil rsaUtil = new RsaUtil(options.PrivateKey, null);
            rsp.Sign = rsaUtil.Sign(signContent);

            return JsonSerializer.Serialize(rsp, Constants.JsonSerializerOption);
        }

        public BasePayRsp MemberApply(string payeeNo)
        {
            IcbcClient client = new IcbcClient(options.BizAppId, options.BizPrivateKey, options.BizGatewayPublicKey, clientFactory.CreateClient());
            var bizMerInfo = options.BizMerInfos.FirstOrDefault(e => e.AccNo == payeeNo) ?? throw new Exception("无效的收款账户！");

            var bizContent = new MybankPayCpayMemberapplyRequestV1.MybankPayCpayMemberapplyRequestV1Biz
            {
                AgreeCode = bizMerInfo.PrtclNo,
                MemberNo = bizMerInfo.Id,
                MemberName = bizMerInfo.AccName,
                MemberType = "01",
                MemberRole = "2",
                CertificateType = "101",
                CertificateId = bizMerInfo.CertificateNo,
                CorpRepreIdType = "0",
                CorpRepreName = bizMerInfo.CorporationName,
                CorpRepreId = bizMerInfo.CorporationNo,
                DealName = bizMerInfo.DealName,
                DealTelphoneNo = bizMerInfo.DealTelNo,
                IcpCode = bizMerInfo.IcpCode,
                AccBankFlag = bizMerInfo.IcbcFlag,
                Accno = bizMerInfo.AccNo,
                AccName = bizMerInfo.AccName,
                AccBankNo = bizMerInfo.AccBankNo,
                AccBankName = bizMerInfo.AccBankName
            };

            var rsp = (MybankPayCpayMemberapplyResponseV1)client.Execute(
                new MybankPayCpayMemberapplyRequestV1 { Content = bizContent });

            var result = new BasePayRsp { Code = rsp.ReturnCode, Message = rsp.ReturnMsg };
            if (rsp.ReturnCode == 0) result.Status = PaymentStatus.Closed;

            return result;
        }
        #endregion
    }
}
