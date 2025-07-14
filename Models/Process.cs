using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace Models
{
    [Table("Processes")]
    public class Process : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ProcessId { get; set; }

        public int? AccountId { get; set; }

        [ForeignKey("UsersId")]
        public virtual LoginViewModel User { get; set; }

        [Required] 
        [MaxLength(255)] 
        public string ProcessName { get; set; }

        public int CurrentStep { get; set; } = 1; 

        public int LastCompletedStep { get; set; } = 0; 
        public int TotalSteps { get; set; }

        [Required]
        public int Status { get; set; } = 0; 

        [Required]
        public DateTime? LastUpdatedDate { get; set; } = DateTime.Now; 

        public int? BuildingId { get; set; }
        public string BuildingName { get; set; }
        [ForeignKey("BuildingId")]
        public virtual Buidings buildings { get; set; }
        public class ProcessFilter
        {
            public int? AccountId { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
            public int? BuildingId { get; set; }
            public string ProcessName { get; set; } 
        }
    }
}