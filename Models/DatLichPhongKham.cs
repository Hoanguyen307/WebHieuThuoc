using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DatLichPhongKham : BaseModel
    {
        public int DatLichId { get; set; }
        public int KhachHangId { get; set; }
        public int DichVuId { get; set; }
        public int? BacSiId { get; set; }
        public DateTime NgayDat { get; set; }
        public string KhungGio { get; set; }
        public string TrangThai { get; set; }
        public string TenKhachHang { get; set; }
        public string TenBacSi { get; set; }
        public DateTime? NgayTao { get; set; }

        [ForeignKey("KhachHangId")]
        public virtual KhachHang KhachHang { get; set; }
        [ForeignKey("DichVuId")]
        public virtual DichVuPhongKham DichVu { get; set; }
        [ForeignKey("BacSiId")]
        public virtual BacSi BacSi { get; set; }
    }
}
