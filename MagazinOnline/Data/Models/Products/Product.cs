using MagazinOnline.Data.Models.Orders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagazinOnline.Data.Models.Products
{
    public class Product
    {
        public Product()
        {
            this.Orders = new List<OrdersProduct>();
        }

        public int Id { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public char Gender { get; set; }

        public byte[] Image { get; set; }
        public int ProductCategoryId { get; set; }


        public virtual ProductCategory ProductType { get; set; }

        public virtual ICollection<OrdersProduct> Orders { get; set; }
    }
}
