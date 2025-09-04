using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("DichVu")]
    public class DichVu : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public int DanhMucId { get; set; }
        public string TenDichVu { get; set; }
        public string TenDanhMuc { get; set; }
        public decimal Gia { get; set; }
        public int ThoiGian { get; set; }
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("DanhMucId")]
        public virtual DichVuPhongKham DichVuPhongKham { get; set; }
        public class DichVuFilter
        {
            public string Name { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
            public int? DanhMucId { get; set; }
            public string CategoryName { get; set; }
            public decimal? MinPrice { get; set; }
            public decimal? MaxPrice { get; set; }
        }
    }
}
