using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PointsModel
    {
        [Table("PointsConfig")]
        public class PointsConfig
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public decimal PointsPerAmount { get; set; }

            [StringLength(255)]
            public string Description { get; set; }

            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public DateTime? UpdatedDate { get; set; }
        }

        [Table("CustomerPoints")]
        public class CustomerPoints
        {
            [Key]
            public int Id { get; set; }

            public int CustomerId { get; set; }
            public decimal PointsBalance { get; set; } = 0;

            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public DateTime? UpdatedDate { get; set; }
            [ForeignKey("CustomerId")]
            public virtual KhachHang Customer { get; set; }
        }

        [Table("PointsHistory")]
        public class PointsHistory
        {
            [Key]
            public int Id { get; set; }

            
            public int CustomerId { get; set; }

            public int? OrderId { get; set; }
            public decimal Points { get; set; } 

            [StringLength(50)]
            public string Type { get; set; } 

            [StringLength(255)]
            public string Description { get; set; }

            public DateTime CreatedDate { get; set; } = DateTime.Now;
            [ForeignKey("CustomerId")]
            public virtual KhachHang Customer { get; set; }
        }
    }
}
