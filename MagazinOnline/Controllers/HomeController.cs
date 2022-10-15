using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MagazinOnline.Data.Context;
using MagazinOnline.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MagazinOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.ProductType);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Statistics()
        {
            var ordersAsync = _context.Orders.Include(o => o.OrderProducts).ToListAsync();
            var orders = ordersAsync.Result;
            ViewData["TotalOrders"] = orders.Count;

            var productsSold = 0;
            foreach(var order in orders)
            {
                foreach(var orderProduct in order.OrderProducts)
                {
                    productsSold += orderProduct.Quantity;
                }
            }
            ViewData["ProductsSold"] = productsSold;

            decimal sales = 0;
            foreach (var order in orders)
            {
                foreach (var orderProduct in order.OrderProducts)
                {
                    sales += orderProduct.Quantity * orderProduct.Product.Price;
                }
            }
            ViewData["Sales"] = sales;

            List<string> days = new List<string>();
            var date = DateTime.UtcNow;


            ViewData["Days"] = GetDaysForChart().ToArray();
            ViewData["SalesOnDays"] = GetSalesOnDaysForChart();

            return View();
        }

        public List<string> GetDaysForChart()
        {
            var months = new string[12] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            List<string> days = new List<string>();
            var date = DateTime.UtcNow;

            for(int i=-9;i < 1; i++)
            {
                var dateTime = date.AddDays(i);
                var dateStr = dateTime.Day.ToString() + " " + months[date.Month];
                days.Add(dateStr);
            }

            return days;
        }

        public List<decimal> GetSalesOnDaysForChart()
        {
            List<decimal> values = new List<decimal>();
            var date = DateTime.UtcNow;

            for (int i = -9; i < 1; i++)
            {
                var dateTime = date.AddDays(i);
                var orders = _context.Orders.Where(o => o.OrderDate.Day == dateTime.Day && o.OrderDate.Month == dateTime.Month && o.OrderDate.Year == dateTime.Year).ToList();
                decimal value = 0;
                foreach (var order in orders)
                {
                    foreach (var orderProduct in order.OrderProducts)
                    {
                        value += orderProduct.Quantity * orderProduct.Product.Price;
                    }
                }
                values.Add(value);
            }

            return values;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
