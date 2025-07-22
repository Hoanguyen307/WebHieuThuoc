using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.LichLamViecViewModel
{
    public class XepLichNhanVienModel
    {
        [Required]
        public int NhanVienId { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public List<LichTrongTuanModel> LichTrongTuan { get; set; }
    }
}
