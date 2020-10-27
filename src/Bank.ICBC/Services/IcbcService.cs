using Bank.Domains.Payment;
using Bank.ICBC.Config;
using Icbc;
using Icbc.Business;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace Bank.Services
{
    public class IcbcService : IPaymentService
    {
        private readonly IcbcOptions options;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<IcbcService> logger;

        //private readonly string merId;
        //private readonly string merPrtclNo;
        //private readonly string appId;
        //private readonly string privateKey;
        //private readonly string gatewayPublicKey;

        private readonly string bizAppId;
        private readonly string bizPrivateKey;
        private readonly string bizGatewayPublicKey;

        public IcbcService(IConfiguration configuration,
            IOptions<IcbcOptions> icbcOptions,
            IHttpClientFactory clientFactory,
            ILogger<IcbcService> logger)
        {
            this.clientFactory = clientFactory;
            this.logger = logger;

            this.bizAppId = configuration.GetValue<string>("Payment:ICBC:BizAppId");
            this.bizPrivateKey = configuration.GetValue<string>("Payment:ICBC:BizPrivateKey");
            this.bizGatewayPublicKey = configuration.GetValue<string>("Payment:ICBC:BizGatewayPublicKey");

            options = icbcOptions.Value;
        }

        public string Pay(Payment payment)
        {
            string result = null;
            switch (payment.PayType)
            {
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cPay(payment);
                    break;
                case PayTypeEnum.Other:
                    result = B2bPay(payment);
                    break;
            }

            return result;
        }

        public string Query(PayQuery payQuery)
        {
            string result = null;
            switch (payQuery.PayType)
            {
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    result = B2cQuery(payQuery);
                    break;
                case PayTypeEnum.Other:
                    result = B2bQuery(payQuery);
                    break;
            }

            return result;
        }
        
        public string Notify(Payment payment)
        {
            switch (payment.PayType)
            {
                case PayTypeEnum.Alipay:
                    throw new NotImplementedException();
                case PayTypeEnum.WechatPay:
                    B2cPay(payment);
                    break;
                case PayTypeEnum.Other:
                    B2bPay(payment);
                    break;
            }
            throw new NotImplementedException();
        }

        #region 聚合支付
        private string B2cPay(Payment payment)
        {
            IcbcClient client = new IcbcClient(options.AppId, options.PrivateKey, options.GatewayPublicKey, clientFactory.CreateClient());
            var merInfo = options.MerInfos.FirstOrDefault(m => m.Id == "100165134848") ?? throw new Exception("无效的Merinfo信息!");

            var bizContent = new CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1.CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1Biz()
            {
                MerId = merInfo.Id,
                OutTradeNo = payment.OrderNo,    //"test20200903002"
                PayMode = payment.PayType == PayTypeEnum.WechatPay ? "9" : "",
                AccessType = payment.PayType == PayTypeEnum.WechatPay ? "9" : "",
                MerPrtclNo = merInfo.PrtclNo,
                OrigDateTime = "2020-09-03T09:10:03",
                DeciveInfo = "013467007045764",  //设备号
                Body = payment.Note,
                FeeType = "001",
                SpbillCreateIp = "192.168.0.1",
                TotalFee = payment.Amount.ToString(),
                MerUrl = payment.NotifyUrl,//"https://www.cptech.com.cn/pay/eventreceive",
                ShopAppid = "wxf113685df9e783f5",
                IcbcAppId = options.AppId,//"10000000000003162305",
                OpenId = payment.Payer,
                NotifyType = "HS",
                ResultType = "0",
                PayLimit = "no_credit"
            };

            var response = (CardbusinessAggregatepayB2cOnlineConsumepurchaseResponseV1)client.Execute(
                new CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1()
                {
                    Content = bizContent
                });

            if (response.ReturnCode.ToString().Equals("0"))
            {
                // 6、业务成功处理，请根据接口文档用response.getxxx()获取同步返回的业务数据
                Console.WriteLine("ReturnCode:" + response.ReturnCode);
                Console.WriteLine("response:" + JsonSerializer.Serialize(response));
                return JsonSerializer.Serialize(response, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            else
            {
                // 失败
                Console.WriteLine("ReturnCode:" + response.ReturnCode);
                Console.WriteLine("response:" + JsonSerializer.Serialize(response));
                Console.WriteLine("ReturnMsg:" + response.ReturnMsg);

                throw new Exception(JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
            }
        }

        private string B2cQuery(PayQuery payQuery)
        {
            IcbcClient client = new IcbcClient(options.AppId, options.PrivateKey, options.GatewayPublicKey, clientFactory.CreateClient());
            var merInfo = options.MerInfos.FirstOrDefault(m => m.Id == "100165134848") ?? throw new Exception("无效的Merinfo信息!");

            var bizContent = new CardbusinessAggregatepayB2cOnlineOrderqryRequestV1.CardbusinessAggregatepayB2cOnlineOrderqryRequestV1Biz()
            {
                MerId = merInfo.Id,
                OutTradeNo = payQuery.OrderNo,    //"test20200903002"
                OrderId = null,
                DealFlag = payQuery.QueryType == QueryTypeEnum.Close ? "1" : "0",
                IcbcAppId = options.AppId   //"10000000000003162305"
            };

            var response = (CardbusinessAggregatepayB2cOnlineOrderqryResponseV1)client.Execute(
                new CardbusinessAggregatepayB2cOnlineOrderqryRequestV1()
                {
                    Content = bizContent
                });

            return JsonSerializer.Serialize(response);
        }

        private string B2cNotify(Payment payment)
        {
            return "";
        }
        #endregion

        #region E企付
        private string B2bPay(Payment payment)
        {
            IcbcClient client = new IcbcClient(bizAppId, bizPrivateKey, bizGatewayPublicKey, clientFactory.CreateClient());

            var recvMallInfoList = new List<MybankPayCpayCppayapplyRequestV2.RecvMallInfo>
            {
                new MybankPayCpayCppayapplyRequestV2.RecvMallInfo
                {
                    MallCode = "0200EH0013035",
                    MccCode = "323451",
                    MccName = "1",
                    BusinessLicense = "1",
                    BusinessLicenseType = "0",
                    MallName = "mallName",
                    PayeeCompanyName = "test",
                    PayeeSysflag = "3",
                    PayeeBankCode = "",
                    PayeeHeadBankCode = "123456789120",
                    PayeeAccno = "aaa-111111",
                    PayAmount = "1",
                    Rbankname = "afica bank",
                    PayeeBankCountry = "840",
                    PayeeBankSign = "CHASUS33XXX",
                    PayeeCountry = "840",
                    PayeeAddress = "urtyusastt",
                    Racaddress1 = "urtyusastt",
                    Racaddress2 = "urtyusastt",
                    Racaddress3 = "urtyusastt",
                    Racaddress4 = "urtyusastt",
                    Racpostcode = "",
                    Agentbic = "",
                    PayeePhone = "13117512457",
                    PayeeOrgcode = "1",
                    ReceivableAmount = ""
                }
            };

            var goodsInfoList = new List<MybankPayCpayCppayapplyRequestV2.GoodsInfo>
            {
                new MybankPayCpayCppayapplyRequestV2.GoodsInfo
                {
                    GoodsSubId = "1",
                    GoodsName = "倚天剑",
                    PayeeCompanyName = "张三",
                    GoodsNumber = "1",
                    GoodsAmt = "1",
                    GoodsUnit = "单位"
                }
            };

            var bizContent = new MybankPayCpayCppayapplyRequestV2.MybankPayCpayCppayapplyRequestV2Biz
            {
                AgreeCode = "0100106889060410006041000000237002",
                PartnerSeq = "MQOY25220",
                PayChannel = "1",
                InternationalFlag = "1",
                PayMode = "1",
                ReservDirect = "",
                OperType = "301",
                PayEntitys = "1",
                AsynFlag = "0",
                LogonId = "",
                PayerAccno = "",
                PayerAccname = "",
                PayerFeeAccno = "",
                PayerFeeAccName = "",
                PayerFeeCurr = "",
                PayMemno = "100190013303483",
                Orgcode = "",
                OrderCode = "2019062730002",
                OrderAmount = "1",
                OrderCurr = "1",
                SumPayamt = "1",
                OrderRemark = "订单备注-直接支付-境内",
                RceiptRemark = "回单补充信息备注",
                Purpose = "",
                Summary = "",
                SubmitTime = "20201101" + DateTime.Now.ToString("HHmmss"),
                ReturnUrl = "",
                CallbackUrl = "https://www.cptech.com.cn/pay/eventreceive",
                AgreementId = "34567891",
                InvoiceId = "",
                ManifestId = "",
                AgreementImageId = "",
                EnterpriseName = "",
                PayRemark = "",
                BakReserve1 = "",
                BakReserve2 = "",
                BakReserve3 = "",
                PartnerSeqOrigin = "",
                SumPayamtOrigin = "",
                ReporterName = "",
                ReporterContact = "",
                IdentityMode = "0",
                PayeeList = recvMallInfoList,
                GoodsList = goodsInfoList
            };

            var response = (MybankPayCpayCppayapplyResponseV2)client.Execute(new MybankPayCpayCppayapplyRequestV2()
            {
                Content = bizContent
            });
            Console.WriteLine("JSONObject.toJSONString(response):" + JsonSerializer.Serialize(response));
            if (response.ReturnCode.ToString().Equals("0"))
            {
                // 业务成功处理
                Console.WriteLine("response.getReturnCode():" + response.ReturnCode);
            }
            else
            {
                //失败
                Console.WriteLine("response.getReturnCode():" + response.ReturnCode);
                Console.WriteLine("response.getReturnMsg():" + response.ReturnMsg);
            }

            return JsonSerializer.Serialize(response);
        }

        private string B2bQuery(PayQuery payQuery)
        {
            return "";
        }

        private string B2bNotify(Payment payment)
        {
            return "";
        }
        #endregion
    }
}
