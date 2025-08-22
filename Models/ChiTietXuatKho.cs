using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ChiTietXuatKho : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public int XuatKhoId { get; set; } 
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaXuat { get; set; }
        [ForeignKey("XuatKhoId")]
        public virtual XuatKho XuatKho { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
