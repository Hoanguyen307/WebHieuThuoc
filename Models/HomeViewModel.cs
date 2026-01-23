using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class HomeViewModel
    {
            public List<Product> FlashDeals { get; set; }
            public List<Product> TopSellingProducts { get; set; }
            public List<Product> LatestProducts { get; set; }
            public List<ProductFlashSaleViewModel> FlashSaleProducts { get; set; }
            
    }

    public class ProductFlashSaleViewModel
    {
        [Key]
        public int ThuocId { get; set; }
        public int ProductId { get; set; }
        public string TenThuoc { get; set; }
        public string HinhAnh { get; set; } = "";
        public decimal? GiaGoc { get; set; }
        public decimal? GiaBan { get; set; }
        public string DonViTinh { get; set; } = "";

        public byte? DiscountPercent { get; set; } 
        public decimal? DiscountAmount { get; set; } 
        public decimal? SalePrice { get; set; }
        public int? FlashStock { get; set; }
        public int? SoldQuantity { get; set; }
    }
}
