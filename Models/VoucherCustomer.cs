using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("VoucherCustomers")]
    public class VoucherCustomer
    {
        [Key]
        public int Id { get; set; }

        public int VoucherId { get; set; }
        public int CustomerId { get; set; }

        public DateTime AssignedDate { get; set; }
        public string AssignedBy { get; set; }

        public bool IsUsed { get; set; }
        public DateTime? UsedDate { get; set; }
        [ForeignKey("VoucherId")]
        public virtual Voucher Voucher { get; set; }
        [ForeignKey("CustomerId")]
        public virtual KhachHang Customer { get; set; }
    }

}
