using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("DonHangChiTiet")]
    public class OrderDetail : BaseModel
    {
        [Key]
        public int ID { get; set; }
        //OrderID
        public string OrderCode { get; set; }
        //productID
        public int ProductId { get; set; }
        public string HinhAnh { get; set; }
        public string TenThuoc { get; set; }

        public string FullName { get; set; }
        public string Address { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal LineTotal { get; set; }
        public int TypePayment { get; set; }
        public decimal TotalAmount { get; set; }

        public string Note { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public List<OrderDetail> listProduct { get; set; }


    }

}
