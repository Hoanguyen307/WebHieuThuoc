using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VoucherViewModel
    {
        public int Id { get; set; }
        public int? VoucherCustomerId { get; set; }
        public int CustomerId { get; set; }
        public DateTime AssignedDate { get; set; }
        public string AssignedBy { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedDate { get; set; }

        // Thông tin của Voucher
        public int VoucherId { get; set; }
        public string Code { get; set; }
        public string Percentage { get; set; }   // "Percentage" hoặc "FixedAmount"
        public decimal DiscountValue { get; set; }
        public int Quantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public bool IsSaved { get; set; }
        public int? SelectedVoucherId { get; set; }
    }
}
