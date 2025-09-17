using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("NhaCungCap")]
    public class NhaCungCap : BaseModel
    {
        [Key]
        public int NhaCungCapId { get; set; }
        public string TenNhaCungCap { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
