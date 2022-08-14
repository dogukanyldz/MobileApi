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
        Task<bool> UpdateBasket(BasketModel basket, string userId);

        Task<BasketModel> DeleteBasketById(string userId, int productCode);
        Task<bool> DeleteBasket(string userId);
        Task<bool> SendBasketItem(BasketModel basketItem,string message);
    }
}
