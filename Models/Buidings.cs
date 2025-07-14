using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Buidings : BaseModel
    {
        [Key]
        public int BuildingId { get; set; }

        [Required(ErrorMessage = "Tên tòa nhà không được để trống.")]
        [StringLength(255)]
        public string BuildingName { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ.")]
        public string Email { get; set; } 

        [StringLength(50)]
        public string ContactPhone { get; set; } 

        public string Description { get; set; } 

        public bool IsActive { get; set; } = true; 
    }
}
