using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Dal
{
   public class BasketItem
    {
        
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }

        public decimal Price => UnitPrice * Count;

        public int Count { get; set; }
        public decimal UnitPrice { get; set; }


        public string CurrencyUnit { get; set; }
    }
}
