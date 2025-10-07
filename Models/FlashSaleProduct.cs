using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FlashSaleProduct
    {
        [Key]
        public int Id { get; set; }
        public int FlashSaleId { get; set; }
        public int ThuocId { get; set; }
        public string TenThuoc { get; set; } 
        public string HinhAnh { get; set; } 
        public decimal GiaGoc { get; set; }
        public decimal SalePrice { get; set; }
        public byte? DiscountPercent { get; set; }
        public decimal? FlashPrice { get; set; }
        public bool IsSelected { get; set; }
    }
}
