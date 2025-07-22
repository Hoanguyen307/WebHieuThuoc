using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("LichLamViec")]
    public class LichLamViec : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public int NhanVienId { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        public DateTime? ThoiGian { get; set; }
        [NotMapped]
        public DateTime? NewThoiGian { get; set; }
        [ForeignKey("NhanVienId")]
        public virtual NhanVien nhanvien { get; set; }
        public int? ShiftId { get; set; }
        [NotMapped]
        public string ShiftName { get; set; }
        [ForeignKey("ShiftId")]
        public virtual CaLam Shift { get; set; }
        [NotMapped]
        public string PositionName { get; set; }
        public int? PositionId { get; set; }

        [ForeignKey("PositionId")]
        public virtual Position position { get; set; }
        [NotMapped]
        public int? UsersId { get; set; }
        public string GhiChu { get; set; }

        [ForeignKey("UsersId")]
        public virtual LoginViewModel User { get; set; }
        public class LichLamViecFilter
        {
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public int? PositionId { get; set; }
            public int? ShiftId { get; set; }
        }


    }
}
