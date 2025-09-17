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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Post> BaiViets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CaLam> CaLams { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<LichLamViec> LichLamViecs { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherCustomer> VoucherCustomers { get; set; }
        public DbSet<DungTich> DungTiches { get; set; }
        public DbSet<DungTichSanPham> DungTichSanPhams { get; set; }
        public DbSet<NhapKho> NhapKhos { get; set; }
        public DbSet<XuatKho> XuatKhos { get; set; }
        public DbSet<ChiTietNhapKho> ChiTietNhapKhos { get; set; }
        public DbSet<ChiTietXuatKho> ChiTietXuatKhos { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public DbSet<DiaChiGiaoHang> DiaChiGiaoHangs { get; set; }
    }
}
