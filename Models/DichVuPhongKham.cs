using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DichVuPhongKham : BaseModel
    {
        [Key]
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public string MoTa { get; set; }
        public bool IsActive { get; set; }
    }
}
