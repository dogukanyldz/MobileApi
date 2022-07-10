using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mobile.Dal;
using Mobile.Dal.Services.Abstract;
using Mobile.Entities.Context;
using Mobile.Entities.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mobile.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string UserId = "";
        public BasketController(ApplicationDbContext applicationDbContext, ApplicationDbContext context, IBasketService basketService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _basketService = basketService;
            _httpContextAccessor = httpContextAccessor;
            UserId = "ada36b30-529f-4510-924c-db64a8337bba"; /*_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);*/
        }

        [HttpGet("AddBasketItem")]
        public async Task<IActionResult> AddBasketItem(string barcode, int count)
        {
            var product =  _context.Products.Where(x => x.Barkod == barcode).FirstOrDefault();
            if (product == null) return NotFound("Product is Not Found");
            var basket = await _basketService.GetBasket(UserId);
            var basketItem = new BasketItem { Price = product.BirimFiyatı * count, UnitPrice=product.BirimFiyatı, Count=count, ProductId = product.DepoId, Barcode=product.Barkod, CurrencyUnit= product.BirimFiyatiParaBirimi, ProductName = product.Isim };

           if (basket == null)
                return BadRequest();
            
            if (basket.basketItems.Count > 0)
            {
                if(!basket.basketItems.Any(x=> x.Barcode == barcode))
                    basket.basketItems.Add(basketItem);               
            }
            else { basket.basketItems.Add(basketItem);}

           var result = await _basketService.AddBasket(basket, UserId);
            
            return Ok(result);

        }


        [HttpGet("GetBasket")]
        public async Task<IActionResult> GetBasket()
        {
            var basket = await _basketService.GetBasket(UserId);

            return Ok(new { totalPrice = basket.TotalPrice, items = basket.basketItems });

        }
        [HttpGet("DeleteBasket")]
        public async Task<IActionResult> DeleteBasket(int productId)
        {
             
            var basket = await _basketService.DeleteBasket(UserId, productId);

            return Ok(new { totalPrice = basket.TotalPrice, items = basket.basketItems });

        }

        [HttpPost("SendBasketItem")]
        public async Task<IActionResult> SendBasketItem([FromBody] BasketModel models)
        {


           var result = await _basketService.SendBasketItem(models);
            return Ok(result);

        }
    }
}
