using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.LichLamViecViewModel
{
    public class LichTrongTuanModel
    {
        [Required]
        public DateTime NgayLam { get; set; }

        [Required]
        public int ShiftId { get; set; }
    }
}
