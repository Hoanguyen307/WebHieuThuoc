using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("tb_OrderDetail")]
    public class OrderDetail : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        //OrderID
        [ForeignKey("Order")]
        public int Orderld { get; set; }
        //productID
        [ForeignKey("Product")]
        public int Productld { get; set; }
        public decimal Price { get; set; }

        public int Quantity { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
