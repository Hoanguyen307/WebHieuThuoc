using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("ChatSessions")]
    public class ChatSession
    {
        public int SessionId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
