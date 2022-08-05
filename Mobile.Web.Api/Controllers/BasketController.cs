using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mobile.Dal;
using Mobile.Dal.Services.Abstract;
using Mobile.Entities.Context;
using Mobile.Entities.Entities;
using MongoDB.Driver;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mobile.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IBasketService _basketService;
        private readonly SharedIdentity _sharedIdentity;

        public BasketController(ApplicationDbContext applicationDbContext, ApplicationDbContext context, IBasketService basketService, SharedIdentity sharedIdentity)
        {
            _context = context;
            _basketService = basketService;
            _sharedIdentity = sharedIdentity;
        }

        [HttpGet("AddBasketItem")]
        public async Task<IActionResult> AddBasketItem(string barcode, int count)
        {
            var product =  _context.Products.Where(x => x.Barkod == barcode).FirstOrDefault();
            if (product == null) return NotFound("Product is Not Found");
            var basket = await _basketService.GetBasket(_sharedIdentity.UserId);
            var basketItem = new BasketItem {UnitPrice=product.BirimFiyatı, Count=count, ProductId = product.DepoId, Barcode=product.Barkod, CurrencyUnit= product.BirimFiyatiParaBirimi, ProductName = product.Isim };
           if (basket == null)
                return BadRequest();
            
            if (basket.basketItems.Any())
            {
                if(!basket.basketItems.Any(x=> x.Barcode == barcode))
                    basket.basketItems.Add(basketItem);               
            }
            else { basket.basketItems.Add(basketItem);}

           var result = await _basketService.AddBasket(basket, _sharedIdentity.UserId);


            return Ok(result);

        }

        [HttpGet("GetBasket")]
        public async Task<IActionResult> GetBasket()
        {
            var basket = await _basketService.GetBasket(_sharedIdentity.UserId);

            return Ok(new { totalPrice = basket.TotalPrice, items = basket.basketItems });

        }
        [HttpGet("UpdateBasket")]
        public async Task<IActionResult> UpdateBasket(string barcode, int count )
        {
            var basket = await _basketService.GetBasket(_sharedIdentity.UserId);

            var basketItems = basket.basketItems.Where(x => x.Barcode == barcode).FirstOrDefault();
            if (basketItems is null)
                return NotFound();

            basketItems.Count = count;

            await _basketService.UpdateBasket(basket, _sharedIdentity.UserId);
            return Ok(new { totalPrice = basket.TotalPrice, items = basket.basketItems });

        }

        [HttpGet("DeleteBasketById")]
        public async Task<IActionResult> DeleteBasketById(int productId)
        {
             
            var basket = await _basketService.DeleteBasketById(_sharedIdentity.UserId, productId);

            return Ok(new { totalPrice = basket.TotalPrice, items = basket.basketItems });

        }
        [HttpDelete("DeleteBasket")]
        public async Task<IActionResult> DeleteBasket()
        {
             
            var basket = await _basketService.DeleteBasket(_sharedIdentity.UserId);

            return Ok(new { result = basket });

        }
        
        [HttpPost("SendBasketItem")]
        public async Task<IActionResult> SendBasketItem([FromBody] BasketModel models)
        {

           var result = await _basketService.SendBasketItem(models);
            return Ok(result);

        }
    }
}
