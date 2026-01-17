using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("DonHang")]
    public class Order : BaseModel
    {
        [Key]
        public int ID { get; set; }
        public int DiaChiId { get; set; }
        [Required]
        public string OrderCode { get; set; }
        [NotMapped]
        public string CustomerName { get; set; }
        [NotMapped]
        public string Phone { get; set; }
        /*public string Address { get; set; }
        public string Email { get; set; }
        public int Quantity { get; set; }*/
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public int? OrderType { get; set; }
        public string HinhAnhDonThuoc { get; set; }
        [NotMapped]
        public string DeliveryServiceName { get; set; }
        [NotMapped]
        public string DriverName { get; set; }
        [NotMapped]
        public string WarehouseName { get; set; }
        [NotMapped]
        public string CarrierName { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        //public virtual ICollection<ThongKe> ThongKes { get; set; }
        //public virtual ICollection<Product> Products { get; set; }

        public class OrderFilter
        {
            public string Keyword { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
            public string Status { get; set; }
        }

    }
}
