using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("Products")]
    public class Product : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }
        [NotMapped]
        public string Detail { get; set; }

        public int ProductCategoryId { get; set; }
        [NotMapped]
        public string ProductCategoryName { get; set; }

        public string Image { get; set; }

        public decimal? Price { get; set; }

        public decimal? SalePrice { get; set; } 
        public int Quantity { get; set; }
        public int Sold { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; }
        [NotMapped]
        public string Tags { get; set; }
        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory ProductCategory { get; set; }
        public int? BrandId { get; set; }
        [NotMapped]
        public string BrandName { get; set; }
        [ForeignKey("BrandId")]
        public virtual ThuongHieu Brand { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        //public virtual ICollection<Wishlist> Wishlists { get; set; }
        /*[ForeignKey("OrderId")]
        public virtual Order Order { get; set; }*/

        public class ProductFilter
        {
            public string Name { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
            public int? ProductCategoryId { get; set; }
            public int? BrandId { get; set; }
            public string CategoryName { get; set; }
            public decimal? MinPrice { get; set; }
            public decimal? MaxPrice { get; set; }
        }
    }

    [Table("ProductReviews")]
    public class ProductReview
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        
        public int CustomerId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("CustomerId")]
        public virtual KhachHang Customer { get; set; }
    }
}
