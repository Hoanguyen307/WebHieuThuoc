using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BacSi : BaseModel
    {
        public int BacSiId { get; set; }
        public string HoTen { get; set; }
        public string ChuyenMon { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
