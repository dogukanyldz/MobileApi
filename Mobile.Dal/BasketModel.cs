using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Dal
{
    public class BasketModel
    {
        public BasketModel()
        {
            basketItems = new();
        }

        public List<BasketItem> basketItems { get; set; }

        public decimal TotalPrice => basketItems.Sum(x => x.Price);
       
    }
}

    

