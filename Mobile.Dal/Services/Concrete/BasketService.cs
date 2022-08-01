using Mobile.Dal.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using Mobile.Entities.Entities;
using Microsoft.Extensions.Options;
using System.IO;
using Mobile.Entities.Context;
using Microsoft.EntityFrameworkCore;

namespace Mobile.Dal.Services.Concrete
{
    public class BasketService: IBasketService
    {
        private readonly RedisService _redis;
        private readonly SmtpSettings _smtpSettings;
        private readonly ApplicationDbContext _applicationDbContext;
        public BasketService(RedisService redis, IOptions<SmtpSettings> smtpSettings, ApplicationDbContext applicationDbContext)
        {
            _redis = redis;
            _smtpSettings = smtpSettings.Value;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<BasketModel> GetBasket(string userId)
        {
            var redisResult = await _redis.GetDb().StringGetAsync(userId);
            if (redisResult.IsNull)
                return new BasketModel();
            
            var basket = JsonConvert.DeserializeObject<BasketModel>(redisResult);


            return basket;

        }
        public async Task<bool> AddBasket(BasketModel basket, string userId)
        {
            var redisResult = await _redis.GetDb().StringSetAsync(userId,JsonConvert.SerializeObject(basket));
            
            return redisResult;

        }

        public async Task<BasketModel> DeleteBasketById(string userId, int productId)
        {
            var redisResult = await GetBasket(userId);            
            if (redisResult.basketItems.Count > 0)
            {
                var deletedItem = redisResult.basketItems.Where(x => x.ProductId == productId).FirstOrDefault();
                if (deletedItem == null)
                    return new BasketModel();

                redisResult.basketItems.Remove(deletedItem);
               
                await _redis.GetDb().StringSetAsync(userId, JsonConvert.SerializeObject(redisResult));

                return redisResult;
            }

            return new BasketModel();
        }  
        public async Task<bool> DeleteBasket(string userId)
        {
            var redisResult = await GetBasket(userId);            
            if (redisResult.basketItems.Count > 0)
            {              
                redisResult.basketItems.Clear();
               
                await _redis.GetDb().StringSetAsync(userId, JsonConvert.SerializeObject(redisResult));

            }

            return true;
        }


        public async Task<bool> SendBasketItem(BasketModel basket)
        {
            SmtpClient smtp = new()
            {
                Host = _smtpSettings.Host,
                Port = _smtpSettings.Port,
                EnableSsl = _smtpSettings.EnableSsl,
                UseDefaultCredentials = _smtpSettings.UseDefaultCredentials,
                Credentials = new NetworkCredential(_smtpSettings.From, _smtpSettings.Password)
            };
            MailMessage mailMessage = new(_smtpSettings.From, _smtpSettings.To);
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = "Sipariş Raporu";
            var path = Directory.GetCurrentDirectory();
            string html = _applicationDbContext.Template.AsNoTracking().Select(x => x.TemplateFile).FirstOrDefault();
            StringBuilder builder = new();
            builder.Append(html);
            basket.basketItems.ForEach(x => {
                
              builder.Append($"<tr> <td>{x.ProductName} </td> <td>{x.Barcode} </td> <td>{x.ProductId} </td> <td>{x.CurrencyUnit} </td> <td>{x.Count} </td> <td>{x.UnitPrice} </td> </tr>");
            
            });
            builder.Append($"<tr> <td> </td> <td></td> <td> </td> <td> </td> <td> </td> <td> Toplam Tutar : {basket.TotalPrice} EUR </td> </tr>");

            builder.Append("</table> </body> </html>");
            mailMessage.Body = builder.ToString();

            try
            {
                await smtp.SendMailAsync(mailMessage);

            }
            catch (Exception) {return false;}
    
            return true;
        }
    }
}
