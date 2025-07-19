using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("BaiViet")]
    public class Post : BaseModel
    {
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string AnhDaiDien { get; set; }
        public bool TrangThai { get; set; }

        public class PostFilter
        {
            public string TieuDe { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
        }
    }
}
