using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("Employees")]
    public class NhanVien : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(255)]
        public string FullName { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(255)]
        public string Email { get; set; }
        public decimal? Salary { get; set; }

        public bool Gender { get; set; } // true: Nam, false: Nữ

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        public int? ShiftId { get; set; }
        public string ShiftName { get; set; }
        [ForeignKey("ShiftId")]
        public virtual CaLam Shift { get; set; }
        public string PositionName { get; set; }
        public int? PositionId { get; set; }

        [ForeignKey("PositionId")]
        public virtual Position position { get; set; }

        public int? UsersId { get; set; }

        [ForeignKey("UsersId")]
        public virtual LoginViewModel User { get; set; }

        public virtual ICollection<LichSu_ChucVu> LichSuChucVu { get; set; }

        public class NhanVienFilter
        {
            public string FullName { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
            public string PositionName { get; set; }
            public int? PositionId { get; set; }
            public int? ShiftId { get; set; }
            public string ShiftName { get; set; }
        }
    }
}
