using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("NhanVien_ChucVu")]
    public class LichSu_ChucVu : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public int NhanVienId { get; set; }

        [ForeignKey("NhanVienId")]
        public virtual NhanVien NhanVien { get; set; }
        public int PositionId { get; set; }
        public string PositionName { get; set; }

        [ForeignKey("PositionId")]
        public virtual Position Position { get; set; }

        [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DenNgay { get; set; }
    }
}
