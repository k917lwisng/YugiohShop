using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YugiohShop
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string CardCode { get; set; }
        public string Name { get; set; }
        public decimal SellPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }

        public decimal LineTotal => SellPrice * Quantity;

    }
}