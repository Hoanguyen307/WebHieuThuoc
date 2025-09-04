using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("ChatMessage")]
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }
        public int SessionId { get; set; }
        public int? SenderId { get; set; }
        public string Message { get; set; }
        public string SenderType { get; set; }
        public DateTime SentDate { get; set; }
        [ForeignKey("SenderId")]
        public virtual KhachHang KhachHang { get; set; }
    }
}
