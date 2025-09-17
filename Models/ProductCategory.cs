using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("ProductCategory")]
    public class ProductCategory : BaseModel
    {
        [Key]
        public int DanhMucId { get; set; }
        public string TenDanhMuc { get; set; } = "";
        public int? ParentId { get; set; } // null = root
        public string MoTa { get; set; } = "";
        public string SeoSlug { get; set; } = "";
        public string CategoryName { get; set; }
        public int ThuTuHienThi { get; set; } = 0;
        public int Category_ID { get; set; } 
        public bool KichHoat { get; set; } = true;

        // Navigation
        public virtual Category Parent { get; set; }
        public virtual ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();
        public virtual ICollection<Product> Thuocs { get; set; } = new List<Product>();
    }
}
