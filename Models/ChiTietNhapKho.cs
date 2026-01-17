using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("NhapKhoChiTiet")]
    public class ChiTietNhapKho : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public int NhapKhoId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHH { get; set; }
        public int? HanSuDung { get; set; }
        public string GhiChu { get; set; }
        [ForeignKey("NhapKhoId")]
        public virtual NhapKho NhapKho { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
