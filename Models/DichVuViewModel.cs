using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DichVuViewModel
    {
        public string Sort { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        
        [Table("DatLichPhongKham")]
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
            public virtual DichVuPhongKham DichVuPhongKham { get; set; }
            [ForeignKey("BacSiId")]
            public virtual BacSi BacSi { get; set; }
        }

        [Table("DichVuPhongKham")]
        public class DichVuPhongKham : BaseModel
        {
            public int DichVuId { get; set; }
            public string TenDichVu { get; set; }
            public string MoTa { get; set; }
            public bool IsActive { get; set; }

            
        }

        [Table("DichVu")]
        public class DichVu : BaseModel
        {
            public int DanhMucId { get; set; }
            public string TenDichVu { get; set; }
            public decimal Gia { get; set; }
            public int ThoiGian { get; set; }
            public string MoTa { get; set; }
            public string HinhAnh { get; set; }
            public bool IsActive { get; set; }
            [ForeignKey("DanhMucId")]
            public virtual DichVuPhongKham DichVuPhongKham { get; set; }
            public class DichVuPhongKhamFilter
            {
                public string Name { get; set; }
                public int? Month { get; set; }
                public int? Year { get; set; }
                public int? ProductCategoryId { get; set; }
                public int? BrandId { get; set; }
                public string CategoryName { get; set; }
                public decimal? MinPrice { get; set; }
                public decimal? MaxPrice { get; set; }
            }
        }
    }
}
