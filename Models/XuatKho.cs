using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("XuatKho")]
    public class XuatKho : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string MaPhieu { get; set; }
        public int? OrderId { get; set; }
        public DateTime NgayXuat { get; set; }
        public string NguoiXuat { get; set; }
        public string LyDo { get; set; }
        public string GhiChu { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual List<ChiTietXuatKho> ChiTietXuatKho { get; set; } = new List<ChiTietXuatKho>();

        public class ChiTietXuatKhoType
        {
            public int ProductId { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGiaXuat { get; set; }
        }
        public class XuatKhoFilter
        {
            public int? Month { get; set; }
            public int? Year { get; set; }
            public string MaPhieu { get; set; }

        }
    }
}
