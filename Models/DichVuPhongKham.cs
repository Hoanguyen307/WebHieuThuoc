using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DichVuPhongKham : BaseModel
    {
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public decimal Gia { get; set; }
        public int ThoiGian { get; set; } // phút
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }
        public bool IsActive { get; set; }
    }
}
