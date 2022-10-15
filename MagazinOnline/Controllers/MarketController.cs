using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MagazinOnline.Data.Context;
using MagazinOnline.Data.Models.Products;

namespace MagazinOnline.Controllers
{
    public class MarketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Market
        public async Task<IActionResult> Index(string? filter)
        {
            var applicationDbContext = _context.Products.Include(p => p.ProductType);
            ViewData["Filter"] = "All";

            if (filter == "K")
            {
                applicationDbContext = _context.Products.Where(p => p.Gender == Convert.ToChar('K')).Include(p => p.ProductType);
                ViewData["Filter"] = "K";
            } else if (filter == "M")
            {
                applicationDbContext = _context.Products.Where(p => p.Gender == Convert.ToChar('M')).Include(p => p.ProductType);
                ViewData["Filter"] = "M";
            } else if (filter == "W")
            {
                applicationDbContext = _context.Products.Where(p => p.Gender == Convert.ToChar('F')).Include(p => p.ProductType);
                ViewData["Filter"] = "W";
            }


            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        [Route("Add/{id}")]
        public IActionResult Add()
        {
            int id = 2;
            var shopList = HttpContext.Session.Get<List<int>>(SessionHelper.ShoppingCart);

            if (shopList == null)
                shopList = new List<int>();

            if (!shopList.Contains(id))
                shopList.Add(id);

            HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);

            return View("Index");

        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket(int productId, int quantity)
        {
            var shopList = HttpContext.Session.Get<Dictionary<int, int>>(SessionHelper.ShoppingCart);
            if (shopList == null)
            {
                shopList = new Dictionary<int, int>();
            }
            if (shopList.ContainsKey(productId))
            {
                shopList[productId] = shopList[productId] + quantity;
            }
            else
            {
                shopList[productId] = quantity;
            }

            HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);

            var applicationDbContext = _context.Products.Include(p => p.ProductType);

            return View("Index", await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        [Route("Remove/{id}")]
        public IActionResult Remove(int id)
        {
            var shopList = HttpContext.Session.Get<List<int>>(SessionHelper.ShoppingCart);

            if (shopList.Contains(id))
                shopList.Remove(id);

            HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);

            return View();
        }

        // GET: Market/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Market/Create
        public IActionResult Create()
        {
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id");
            return View();
        }

        // POST: Market/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,IsAvailable,Image,ProductCategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", product.ProductCategoryId);
            return View(product);
        }

        // GET: Market/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", product.ProductCategoryId);
            return View(product);
        }

        // POST: Market/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,IsAvailable,Image,ProductCategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", product.ProductCategoryId);
            return View(product);
        }

        // GET: Market/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Market/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
