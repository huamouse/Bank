using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bank.Domains.Payment
{
    public class OrderCreateDto
    {
        [Description("平台方标识")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string Tag { get; set; }

        [Description("收款方")]
        public string Payee { get; set; }

        [Description("支付金额")]
        public decimal Amount { get; set; }

        [Description("支付事项")]
        [MaxLength(100)]
        public string Note { get; set; }

        public long? CreatorId { get; set; }

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