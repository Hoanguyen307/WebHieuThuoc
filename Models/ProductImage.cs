using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("ProductImage")]
    public class ProductImage : BaseModel
    {
        [Key]
        public int HinhAnhId { get; set; }
        public int ThuocId { get; set; }

        public string Url { get; set; } = "";
        public string FileName { get; set; } = "";
        public bool IsDefault { get; set; } = false;
        public int ThuTu { get; set; } = 0;
        public string AltText { get; set; } = "";
        public bool KichHoat { get; set; } = true;

        // Navigation
        public virtual Product Thuoc { get; set; }
    }
}
