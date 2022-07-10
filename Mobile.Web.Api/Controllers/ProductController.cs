using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mobile.Entities.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mobile.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(string barcode)
        {
            var products = await _context.Products.AsNoTracking().Where(x => x.Barkod == barcode).FirstOrDefaultAsync();
            return Ok(products);
        }
    }
}
