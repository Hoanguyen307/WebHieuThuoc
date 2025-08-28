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
        public List<DungTichSanPham> dungTichSanPhams { get; set; }
        public List<ProductReview> productReviews { get; set; }
        public double AverageRating => productReviews != null && productReviews.Any()
        ? productReviews.Average(r => r.Rating)
        : 0;

        public int TotalReviews => productReviews?.Count ?? 0;

    }
}
