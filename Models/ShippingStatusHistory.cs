using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ShippingStatusHistory
    {
        public int Id { get; set; }
        public int ShippingOrderId { get; set; }

        public int Status { get; set; }          // 0–5
        public string Location { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }

        // Optional navigation
        public ShippingOrder ShippingOrder { get; set; }
    }
}
