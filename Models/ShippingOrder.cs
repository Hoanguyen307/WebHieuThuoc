using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ShippingOrder
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public int? DeliveryServiceId { get; set; }
        public int? DriverId { get; set; }
        public int? WarehouseId { get; set; }

        public string TrackingCode { get; set; }

        public int CurrentStatus { get; set; }   // 0–5
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Optional navigations
        public DeliveryService DeliveryService { get; set; }
        public Driver Driver { get; set; }
        public KhoHang Warehouse { get; set; }
    }
}
