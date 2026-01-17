using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        //public List<DungTichSanPham> dungTichSanPhams { get; set; }
        public List<ProductReview> productReviews { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<DiaChiGiaoHang> Addresses { get; set; }
        public int? SelectedAddressId { get; set; }

    }
}
