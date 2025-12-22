using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DeliveryService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Hotline { get; set; }
        public bool IsActive { get; set; }
    }
}
