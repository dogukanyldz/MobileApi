using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Dal.Services.Abstract
{
    public interface IBasketService
    {

        Task<BasketModel> GetBasket(string userId);

        Task<bool> AddBasket(BasketModel basket, string userId);
        Task<BasketModel> DeleteBasket(string userId, int productId);
        Task<bool> SendBasketItem(BasketModel basketItem);
    }
}
