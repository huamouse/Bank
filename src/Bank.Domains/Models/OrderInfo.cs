using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.EFCore.Models
{
    public partial class OrderInfo
    {
        public string InterfaceName { get; set; }
        public string InterfaceVersion { get; set; }
		public string INTERFACE_NAME { get; set; }  // 接口名称
		public string INTERFACE_VERSION { get; set; }  // 接口版本号
		public string ORDER_DATE { get; set; }  //订单日期，格式yyyyMMddHHmmss
		public string ORDER_ID { get; set; }							//订单号, 30位，数字
		public string AMOUNT { get; set; }							//订单金额
		public string INSTALLMENT_TIMES { get; set; }				//分期期数（聚合支付不需要，可以不赋值）
		public string CUR_TYPE { get; set; }						//币种
		public string MER_ID { get; set; }						//商城代码
		public string MER_ACCT { get; set; }							//商城账号
		public string EXPIRE_TIME { get; set; }				//交易过期时间
		public string UNIPASS_ID { get; set; }						//为空，不需要使用
		public string SHOPAPP_ID { get; set; }						//微信必须有值，支付宝可以没有值
		public string ICBC_APP_ID { get; set; }
		public string VERIFY_JOIN_FLAG { get; set; }					//联名校验标志
		public string LANGUAGE { get; set; }					//语言
		public string GOODS_ID { get; set; }							//商品ID
		public string GOODS_NAME { get; set; }						//商品名称
		public string GOODS_NUM { get; set; }							//商品数量
		public string CARRIAGE_AMT { get; set; }					//运费
		public string MER_HINT { get; set; }						//商城提示
		public string REMARK1 { get; set; }						//备注1
		public string REMARK2 { get; set; }                        //备注2
		public string MER_URL { get; set; }    //商城通知地址
		public string MER_VAR { get; set; }						//商城备注
		public string NOTIFY_TYPE { get; set; }					//通知类型
		public string RESULT_TYPE { get; set; }						//通知结果类型
		public string LIMIT_PAY { get; set; }
		public string BACKUP1 { get; set; }						//备用1
		public string BACKUP2 { get; set; }						//备用2
		public string BACKUP3 { get; set; }						//备用3
		public string BACKUP4 { get; set; }						//备用4
		public string IS_SUPPORT_DISCOUFLAG { get; set; }        //支付通知是否支持优惠信息
		public string THIRD_PARTY_FLAG { get; set; }               //支付通知是否返回支付方式标志
		public string MER_PRTCL_NO { get; set; } 	 //外部协议编号
		public string APP_NAME { get; set; }                //APP名称
		public string RETURN_URL { get; set; }                     //回调地址
		public string CLIENT_TYPE{ get; set; }        
    }
}
