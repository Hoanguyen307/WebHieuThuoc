using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        public string MenuName { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
