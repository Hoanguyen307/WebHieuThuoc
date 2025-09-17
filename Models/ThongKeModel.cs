using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ThongKeModel
    {

    }
    public class ThongKeTongQuan
    {
        public decimal TongLoiNhuan { get; set; }
        public int PhanTramThayDoiLoiNhuan { get; set; }
        public int TongDonHang { get; set; }
        public int PhanTramThayDoiDonHang { get; set; }
        public int TongNguoiDung { get; set; }
        public int PhanTramThayDoiNguoiDung { get; set; }
    }

    public class DuLieuBanHang
    {
        public List<string> NhanThoiGian { get; set; }
        public List<decimal?> BanHang { get; set; } 
        public List<decimal?> LoiNhuan { get; set; } 
    }


    public class TongKetTaiChinh
    {
        public decimal TongBanHang { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal ChiPhi { get; set; }
        public decimal TangTruong { get; set; }
    }

}
