using MagazinOnline.Data.Models.Products;

namespace MagazinOnline.Data.Models.Orders
{
    public partial class OrdersProduct
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }

        public int Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        
    }
}
