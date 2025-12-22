using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Driver
    {
        public int Id { get; set; }
        public int DeliveryServiceId { get; set; }
        public string DriverName { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        // Optional: navigation (nếu bạn muốn)
        public DeliveryService DeliveryService { get; set; }
    }
}
