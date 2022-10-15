using MagazinOnline.Data.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagazinOnline.Data.Models.Orders
{
    public class Order
    {
        public Order()
        {
            OrderProducts = new List<OrdersProduct>();
        }

        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual ICollection<OrdersProduct> OrderProducts { get; set; }
    }
}
