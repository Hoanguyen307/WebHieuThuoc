using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("KhachHang")]
    public class KhachHang : BaseModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public bool Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PasswordHash { get; set; }
        public string LockReason { get; set; }
        public string LockBy { get; set; }
        public DateTime? LockTime { get; set; }
        [NotMapped]
        public bool IsMember { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public int SkipCount { get; set; }
        [NotMapped]
        public int MaxResultCount { get; set; }
        [NotMapped]
        public decimal PointsBalance { get; set; }
        public virtual ICollection<DiaChiGiaoHang> DiaChiGiaoHangs { get; set; }
        public class KhachHangFilter
        {
            public string FullName { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
        }
    }
    [Table("DiaChiGiaoHang")]
    public class DiaChiGiaoHang
    {
        [Key]
        public int DiaChiId { get; set; }
        public int KhachHangId { get; set; }

        public string TenNguoiNhan { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChiChiTiet { get; set; }
        public string GhiChu { get; set; }
        public bool MacDinh { get; set; }
        public DateTime NgayTao { get; set; }
        [ForeignKey("KhachHangId")]
        public virtual KhachHang KhachHang { get; set; }
    }

}
