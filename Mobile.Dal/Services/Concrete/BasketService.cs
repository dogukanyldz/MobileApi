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
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;

namespace Mobile.Dal.Services.Concrete
{
    public class BasketService: IBasketService
    {
        private readonly ConnectionFactory _redis;
        private readonly SmtpSettings _smtpSettings;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _env;

        public BasketService(ConnectionFactory redis, IOptions<SmtpSettings> smtpSettings, ApplicationDbContext applicationDbContext, IWebHostEnvironment env)
        {
            _redis = redis;
            _smtpSettings = smtpSettings.Value;
            _applicationDbContext = applicationDbContext;
            _env = env;
        }

        public async Task<BasketModel> GetBasket(string userId)
        {
            var redisResult = await _redis.GetConnection().StringGetAsync(userId);
            if (redisResult.IsNull)
                return new BasketModel();
            
            var basket = JsonConvert.DeserializeObject<BasketModel>(redisResult);


            return basket;

        }
        public async Task<bool> AddBasket(BasketModel basket, string userId)
        {
            var redisResult = await _redis.GetConnection().StringSetAsync(userId,JsonConvert.SerializeObject(basket));
            
            return redisResult;

        }
        public async Task<bool> UpdateBasket(BasketModel basket, string userId)
        {
            var redisResult = await _redis.GetConnection().StringSetAsync(userId, JsonConvert.SerializeObject(basket));

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
               
                await _redis.GetConnection().StringSetAsync(userId, JsonConvert.SerializeObject(redisResult));

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
               
                await _redis.GetConnection().StringSetAsync(userId, JsonConvert.SerializeObject(redisResult));

            }

            return true;
        }


        public async Task<bool> SendBasketItem(BasketModel basket)
        {
            string root_path = $@"{_env.WebRootPath }\SiparisVerileri.xlsx";
            WriteExcelFile(basket.basketItems, root_path);
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

            var attachment = new Attachment(File.Open(root_path, FileMode.Open), "SiparisVerileri.xlsx");
            mailMessage.Attachments.Add(attachment);
            try
            {
                await smtp.SendMailAsync(mailMessage);

            }
            catch (Exception) {return false;}
    
            return true;
        }

        public static void WriteExcelFile(List<BasketItem> model,string path)
        {
            DataTable table = new DataTable();

            foreach (PropertyInfo info in typeof(BasketItem).GetProperties())
            {
                table.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }

            foreach (BasketItem t in model)
            {
                DataRow row = table.NewRow();

                foreach (PropertyInfo info in typeof(BasketItem).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }
                table.Rows.Add(row);
            }

            if (File.Exists(path))
                File.Delete(path);

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet{ Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                sheets.Append(sheet);

                Row headerRow = new Row();

                List<String> columns = new List<string>();
                foreach (System.Data.DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);

                    Cell cell = new();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column.ColumnName);
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (DataRow dsrow in table.Rows)
                {
                    Row newRow = new ();
                    foreach (String col in columns)
                    {
                        Cell cell = new ();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(dsrow[col].ToString());
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }


                    Row newRows = new();
               
                    Cell cells = new();
                    cells.DataType = CellValues.String;
                    cells.CellValue = new CellValue($"TOPLAM TUTAR : {model.Sum(x=> x.Price)} : EUR. ");
                    newRows.AppendChild(cells);
                

                sheetData.AppendChild(newRows);

                workbookPart.Workbook.Save();
            }
        }
    }
}
