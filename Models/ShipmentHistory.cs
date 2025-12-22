using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("ShipmentHistory")]
    public class ShipmentHistory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Shipment")]
        public int ShipmentId { get; set; }

        public string Status { get; set; }
        public string Location { get; set; }
        public string Note { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        public virtual Shipment Shipment { get; set; }
    }
}
