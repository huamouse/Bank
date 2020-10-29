using System.Collections.Generic;
using System.ComponentModel;

namespace Bank.Domains.Payment
{
    public class OrderCreateDto
    {
        [Description("平台方标识")]
        public string Tag { get; set; }

        [Description("收款方")]
        public string Payee { get; set; }

        [Description("支付金额")]
        public decimal Amount { get; set; }

        [Description("支付事项")]
        public string Note { get; set; }

        [Description("创建人")]
        public string CreateId { get; set; }

        [Description("物品清单")]
        public List<Goods> GoodsList { get; set; }

        public class Goods
        {
            public string Name { get; set; }
            public int Number { get; set; }
            public string Unit { get; set; }
            public decimal Price { get; set; }
        }
    }
}
