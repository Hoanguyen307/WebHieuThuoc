using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("Product")]
    public class Product : BaseModel
    {
        [Key]
        public int ThuocId { get; set; }
        public string TenThuoc { get; set; } = "";
        public string HoatChat { get; set; } = "";
        public string DonViTinh { get; set; } = "";
        public string QuyCach { get; set; } = "";
        public string HinhAnh { get; set; } = "";
        public string ThanhPhan { get; set; } = "";
        public string CongDung { get; set; } = "";
        public string CachSuDung { get; set; } = "";
        [NotMapped]
        public string ProductCategoryName { get; set; }
        [NotMapped]
        public string TenNhaCungCap { get; set; }
        public int? DanhMucId { get; set; }
        public int? NhaCungCapId { get; set; }
        public decimal? GiaGoc { get; set; }
        public decimal? GiaBan { get; set; }
        public bool KichHoat { get; set; }
        public bool ThuocKeDon { get; set; }
        public int SoLuong { get; set; }

        public virtual ProductCategory DanhMuc { get; set; }
        public virtual NhaCungCap NhaCungCap { get; set; }
        public virtual ICollection<ProductImage> HinhAnhs { get; set; } = new List<ProductImage>();
        public class ProductFilter
        {
            public string Name { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
            public int? ProductCategoryId { get; set; }
            public int? NhaCungCapId { get; set; }
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

        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string FullName { get; set; }
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

        public class ProductReviewFilter
        {
            public string SearchString { get; set; }
            public int? Rating { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }
    }
}
