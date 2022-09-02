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
            if (barcode.ToLower().StartsWith("set"))
            {
                var products = await _context.SetProducts.Include(y => y.Products).Where(x => x.Barcode == barcode).ToListAsync();
                if (!products.Any()) return NotFound("Set is Not Found");
                var basket = await _basketService.GetBasket(_sharedIdentity.UserId);

                foreach (var product in products)
                {
                    if (basket.basketItems.Any())
                    {
                        if (!basket.basketItems.Any(x => x.SetValue==barcode && x.Barcode == product.Products.Barkod))
                            basket.basketItems.Add(new BasketItem { UnitPrice = product.Products.BirimFiyatı,SetValue=barcode, Count = product.Piece, ProductCode = Convert.ToInt32(product.Products.UrunKodu), Barcode = product.Products.Barkod, CurrencyUnit = product.Products.BirimFiyatiParaBirimi, ProductName = product.Products.Isim });
                        else
                        {
                            foreach (var item in basket.basketItems)
                            {
                                if (item.Barcode == product.Products.Barkod && item.SetValue == barcode)
                                {
                                    item.Count += product.Piece;
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                        var item = new BasketItem {                UnitPrice = product.Products.BirimFiyatı, SetValue = barcode, Count = product.Piece, ProductCode = Convert.ToInt32(product.Products.UrunKodu), Barcode = product.Products.Barkod, CurrencyUnit = product.Products.BirimFiyatiParaBirimi, ProductName = product.Products.Isim };
                        basket.basketItems.Add(item);

                    }

                }
                var result = await _basketService.AddBasket(basket, _sharedIdentity.UserId);
                return Ok(result);

            }


            else
            {
                var product = _context.Products.Where(x => x.Barkod == barcode).FirstOrDefault();
                if (product == null) return NotFound("Product is Not Found");
                var basket = await _basketService.GetBasket(_sharedIdentity.UserId);
                var basketItem = new BasketItem { UnitPrice = product.BirimFiyatı, SetValue="", Count = count, ProductCode = Convert.ToInt32(product.UrunKodu), Barcode = product.Barkod, CurrencyUnit = product.BirimFiyatiParaBirimi, ProductName = product.Isim };
                if (basket == null)
                    return BadRequest();

                if (basket.basketItems.Any())
                {
                    if (!basket.basketItems.Any(x => x.Barcode == barcode && x.SetValue==""))
                    {
                        basket.basketItems.Add(basketItem);
                    }
                    else
                    {
                        foreach (var item in basket.basketItems)
                        {
                            if (item.Barcode == barcode && item.SetValue=="")
                                item.Count += count;
                        }

                    }
                }
                else { basket.basketItems.Add(basketItem); }

                var result = await _basketService.AddBasket(basket, _sharedIdentity.UserId);

                return Ok(result);
            }



        }

        [HttpGet("GetBasket")]
        public async Task<IActionResult> GetBasket()
        {
            var basket = await _basketService.GetBasket(_sharedIdentity.UserId);

            return Ok(new { totalPrice = basket.TotalPrice, items = basket.basketItems });

        }
        [HttpGet("UpdateBasket")]
        public async Task<IActionResult> UpdateBasket(string barcode, int count,string setValue)
        {
            var basket = await _basketService.GetBasket(_sharedIdentity.UserId);

            if(setValue is not null)
            {
                var basketItems = basket.basketItems.Where(x => x.Barcode == barcode && x.SetValue==setValue).FirstOrDefault();
                basketItems.Count = count;
            }
            else
            {
                var basketItems = basket.basketItems.Where(x => x.Barcode == barcode && x.SetValue=="").FirstOrDefault();
                basketItems.Count = count;
            }
           

            await _basketService.UpdateBasket(basket, _sharedIdentity.UserId);
            return Ok(new { totalPrice = basket.TotalPrice, items = basket.basketItems });

        }

        [HttpGet("DeleteBasketById")]
        public async Task<IActionResult> DeleteBasketById(int productCode,string setValue)
        {
             
            var basket = await _basketService.DeleteBasketById(_sharedIdentity.UserId, productCode,setValue);

            return Ok(new { totalPrice = basket.TotalPrice, items = basket.basketItems });

        }
        [HttpDelete("DeleteBasket")]
        public async Task<IActionResult> DeleteBasket()
        {
             
            var basket = await _basketService.DeleteBasket(_sharedIdentity.UserId);

            return Ok(new { result = basket });

        }
        
        [HttpPost("SendBasketItem")]
        public async Task<IActionResult> SendBasketItem([FromBody] BasketModel models,string message)
        {

           var result = await _basketService.SendBasketItem(models,message);
            return Ok(result);

        }
    }
}
