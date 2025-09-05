using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("Order")]
    public class Order : BaseModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string OrderCode { get; set; }
        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; }
        public string Email { get; set; }
        public int Quantity { get; set; }
        public int TypePayment { get; set; }
        public decimal TotalAmount { get; set; }
        //lấy thông tin người dùng khi đặt hàng
        public int CustomerId { get; set; }
        public string PaymentStatus { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
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

        public class ShipmentHistory
        {
            [Key]
            public int Id { get; set; }
            public int OrderId { get; set; }
            public string Status { get; set; }
            public string TrackingNumber { get; set; }
            public string Carrier { get; set; }
            public string Location { get; set; }
            public string Note { get; set; }
            public string CreatedBy { get; set; }
            public DateTime UpdatedDate { get; set; }
            public bool IsCurrent { get; set; }
            [ForeignKey("OrderId")]
            public virtual Order Order { get; set; }
        }
    }
}
