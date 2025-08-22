using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TonKhoModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int TongNhap { get; set; }
        public int TongXuat { get; set; }
        public int SoLuongTon { get; set; }

        public DateTime? HanSuDung { get; set; }   

        public bool CanhBaoHetHang { get; set; }
        public bool CanhBaoHetHan { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

}
