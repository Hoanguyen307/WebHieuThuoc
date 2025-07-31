using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("Products")]
    public class Product : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }
        [NotMapped]
        public string Detail { get; set; }

        public int CategoryId { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public decimal? SalePrice { get; set; } // Giá khuyến mãi (có thể null)
        public int Quantity { get; set; }
        public int Sold { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; }
        [NotMapped]
        public string Tags { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category ProductCategory { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        //public virtual ICollection<Wishlist> Wishlists { get; set; }
        /*[ForeignKey("OrderId")]
        public virtual Order Order { get; set; }*/

        public class ProductFilter
        {
            public string Name { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
            public int? CategoryId { get; set; }
            public string CategoryName { get; set; }
            public decimal? MinPrice { get; set; }
            public decimal? MaxPrice { get; set; }
        }
    }
}
