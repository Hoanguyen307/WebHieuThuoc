using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("NhapKho")]
    public class NhapKho : BaseModel
    {
        public int Id { get; set; }
        public string MaPhieu { get; set; }
        public DateTime NgayNhap { get; set; }
        public string NguoiNhap { get; set; }
        public string NhaCungCap { get; set; }
        public string GhiChu { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual List<ChiTietNhapKho> ChiTietNhapKho { get; set; } = new List<ChiTietNhapKho>();
    }

    public class ChiTietNhapKhoType
    {
        public int ProductId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string GhiChu { get; set; }
    }
    public class NhapKhoFilter
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string MaPhieu { get; set; }

    }
}
