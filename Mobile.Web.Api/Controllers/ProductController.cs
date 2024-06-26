﻿using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
        [HttpGet("GetSetProductById")]
        public async Task<IActionResult> GetSetProductById(string barcode)
        {
            var existSetProducts = await _context.SetProducts.AsNoTracking().Include(i=> i.Products).Where(x => x.Barcode == barcode).ToListAsync();

            return Ok(new { item = existSetProducts });
        }
    }
}
