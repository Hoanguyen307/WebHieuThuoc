using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("DungTichSanPham")]
    public class DungTichSanPham
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        public int DungTichId { get; set; } 
        public string DungTichValue { get; set; } 
        public decimal? Gia { get; set; }
        public decimal? SalePrice { get; set; }
        public int SoLuong { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [ForeignKey("DungTichId")]
        public DungTich DungTich { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }

}
