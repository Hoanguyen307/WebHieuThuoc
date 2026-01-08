using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string TieuDe { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public string LoaiDanhMuc { get; set; }
        [Required(ErrorMessage = "Nội dung bài viết không được để trống")]
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
