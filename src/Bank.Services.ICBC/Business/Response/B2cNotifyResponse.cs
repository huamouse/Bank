using System.Text.Json.Serialization;

namespace Icbc.Business
{
    public class B2cNotifyResponse
    {
        [JsonPropertyName("response_biz_content")]
        public BizContent ResponseBizContent { get; set; }

        [JsonPropertyName("sign_type")] 
        public string SignType { get; set; }

        [JsonPropertyName("sign")]
        public string Sign { get; set; }
        
        public class BizContent
        {
            [JsonPropertyName("return_code")]
            public int ReturnCode { get; set; }

            [JsonPropertyName("return_msg")]
            public string ReturnMsg { get; set; }

            [JsonPropertyName("msg_id")]
            public string MsgId { get; set; }
        }
    }
}
