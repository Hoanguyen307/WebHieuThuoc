using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CheckoutViewModel
    {
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

        // Phương thức thanh toán
        public List<PaymentMethodViewModel> PaymentMethods { get; set; }
        public string SelectedPaymentMethod { get; set; }

        // Điểm thưởng (nếu có)
        public int CurrentPoints { get; set; }
        public bool UsePoints { get; set; }
        public int PointsToUse { get; set; }

        // Ghi chú đơn hàng
        public string Note { get; set; }
    }

    public class PaymentMethodViewModel
    {
        public string Code { get; set; }   // VD: "COD", "VNPAY", "MOMO"
        public string Name { get; set; }   // VD: "Thanh toán khi nhận hàng", "VNPay", "Momo"
    }
}
