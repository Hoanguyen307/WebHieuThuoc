using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("DungTich")]
    public class DungTich
    {
        public int Id { get; set; }
        public string Value { get; set; } 
        public DateTime CreatedDate { get; set; }
    }

}
