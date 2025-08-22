using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Models
{
    public partial class DBConnect : DbContext
    {
        public DBConnect()
            : base("name=DBConnect")
        {
        }

        //public virtual DbSet<User> User { get; set; }
        //public virtual DbSet<Role> Role { get; set; }
        //public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<CaLam> CaLams { get; set; }
        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<Post> BaiViets { get; set; }
        //public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<LichLamViec> LichLamViecs { get; set; }
        public virtual DbSet<ThuongHieu> Brands { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<VoucherCustomer> VoucherCustomers { get; set; }
        public virtual DbSet<DungTich> DungTiches { get; set; }
        public virtual DbSet<DungTichSanPham> DungTichSanPhams { get; set; }
        public virtual DbSet<NhapKho> NhapKhos { get; set; }
        public virtual DbSet<XuatKho> XuatKhos { get; set; }
    }
}
