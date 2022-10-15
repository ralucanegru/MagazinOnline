using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagazinOnline.Data.Context;
using MagazinOnline.Data.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagazinOnline.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var shopList = HttpContext.Session.Get<Dictionary<int, int>>(SessionHelper.ShoppingCart);
            if (shopList == null)
            {
                shopList = new Dictionary<int, int>();
            }

            var selectedProductsIds = shopList.Keys.ToList();
            var selectedProducts = _context.Products.Where(x => selectedProductsIds.Contains(x.Id)).ToList();

            Order order = new Order();
            foreach(var product in selectedProducts)
            {
                OrdersProduct op = new OrdersProduct();
                op.Product = product;
                op.ProductId = product.Id;
                order.OrderProducts.Add(op);
            }

            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerName,CustomerPhoneNumber,CustomerEmail,CustomerAddress,OrderDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.UtcNow;
                _context.Add(order);
                await _context.SaveChangesAsync();

                var shopList = HttpContext.Session.Get<Dictionary<int, int>>(SessionHelper.ShoppingCart);

                foreach(var key in shopList.Keys)
                {
                    OrdersProduct op = new OrdersProduct();
                    op.OrderId = order.Id;
                    op.ProductId = key;
                    op.Quantity = shopList[key];
                    _context.OrderProducts.Add(op);
                }
                _context.SaveChanges();

                shopList = new Dictionary<int, int>();

                HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);

                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,CustomerPhoneNumber,CustomerEmail,CustomerAddress,OrderDate")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
