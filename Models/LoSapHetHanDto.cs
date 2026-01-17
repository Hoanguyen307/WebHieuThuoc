using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class LoSapHetHanDto
    {
        public int BatchId { get; set; }
        public string ProductName { get; set; }
        public string MaLo { get; set; }
        public int SoLuongTon { get; set; }
        public DateTime NgaySanXuat { get; set; }
        public DateTime HanSuDung { get; set; }
        public int SoNgayConLai { get; set; }
    }
}
