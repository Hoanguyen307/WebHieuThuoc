using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("Customers")]
    public class KhachHang : BaseModel
    {
        public int Id { get; set; }

        [Required]
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
        public bool IsMember { get; set; }
        public bool IsActive { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public decimal PointsBalance { get; set; }
        public class KhachHangFilter
        {
            public string FullName { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
        }
    }
}
