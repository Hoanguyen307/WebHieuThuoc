using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DoanhThu
    {
        public class DoanhThuTongHop
        {
            public int SoDonHang { get; set; }
            public decimal TongDoanhThu { get; set; }
        }

        public class SanPhamBanChay
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int TongSoLuongBan { get; set; }
            public decimal DoanhThu { get; set; }
        }

        public class BaoCaoDoanhThu
        {
            public DoanhThuTongHop TongHop { get; set; }
            public List<SanPhamBanChay> TopSanPham { get; set; }
        }

    }
}
