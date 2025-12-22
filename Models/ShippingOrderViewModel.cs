using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ShippingOrderViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public int CustomerId { get; set; }

        public string TrackingCode { get; set; }
        public int CurrentStatus { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? DeliveryServiceId { get; set; }
        public string DeliveryServiceName { get; set; }
        public int? DriverId { get; set; }
        public string DriverName { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }

    }
}
