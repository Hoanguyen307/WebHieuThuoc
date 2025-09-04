using System;
using System.Collections.Generic;
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
            public List<DichVuPhongKham> DichVuPhongKhamList { get; set; }
            public List<DichVu> DichVus { get; set; }
            
    }
}
