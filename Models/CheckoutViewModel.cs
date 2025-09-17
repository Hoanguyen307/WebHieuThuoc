using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CheckoutViewModel
    {
        public CheckoutViewModel()
        {
            CartItems = new List<CartItemModel>();
            Addresses = new List<DiaChiGiaoHang>();
            Vouchers = new List<VoucherViewModel>();
            PaymentMethods = new List<PaymentMethodViewModel>();
            Orders = new List<Order>();
        }
        // Giỏ hàng (chỉ để hiển thị, chúng ta sẽ lấy giỏ từ DB trong POST)
        public List<CartItemModel> CartItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalTotal { get; set; }

        // Địa chỉ giao hàng
        public List<DiaChiGiaoHang> Addresses { get; set; }
        public int? SelectedAddressId { get; set; }

        // Voucher
        public List<VoucherViewModel> Vouchers { get; set; }
        public int? SelectedVoucherId { get; set; }

        public List<PaymentMethodViewModel> PaymentMethods { get; set; }
        public string SelectedPaymentMethod { get; set; }
        public string VnPayType { get; set; }

        public int CurrentPoints { get; set; }
        public bool UsePoints { get; set; }
        public int PointsToUse { get; set; }

        public string Note { get; set; }

        public List<Order> Orders { get; set; }
    }

    public class PaymentMethodViewModel
    {
        public string Code { get; set; }   // VD: "COD", "VNPAY", "MOMO"
        public string Name { get; set; }   // VD: "Thanh toán khi nhận hàng", "VNPay", "Momo"
    }
}
