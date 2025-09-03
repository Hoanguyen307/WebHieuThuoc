using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        [Key]
        public int ProductId { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public int CartCount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
        public bool Status { get; set; }
    }

}
