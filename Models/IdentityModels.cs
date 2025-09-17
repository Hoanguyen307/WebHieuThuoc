using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Models;
//using Migrations;

namespace Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime? CreatedDate { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(100)]
        public string UpdatedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        [StringLength(100)]
        public string DeletedBy { get; set; }

        public bool IsDeleted { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

            // User/Role có sẵn từ IdentityDbContext
            public DbSet<Category> Categories { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<ProductImage> ProductImages { get; set; }
            public DbSet<ProductCategory> ProductCategories { get; set; }
            public DbSet<Post> Posts { get; set; }
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

            public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}