using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Models.DanhMuc.TheTraTruoc;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using APIServices.DanhMuc.TheTraTruoc;

namespace PayOSDemo.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PayOS _payOS;
        private readonly IConfiguration _config;
        private readonly IDictionary<long, string> _store;
        private readonly ITheTraTruocService _theTraTruocService;
        public PaymentController(PayOS payOS, IConfiguration config, ITheTraTruocService theTraTruocService, IDictionary<long, string> store)
        {
            _theTraTruocService = theTraTruocService;
            _payOS = payOS;
            _config = config;
            _store = store;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Return(long orderCode, string? status = null, string? code = null, string? id = null, bool? cancel = null, string IdThe = null, decimal? SoTien = null, string? GhiChu = null, string? Code = null)
        {
            _store.TryGetValue(orderCode, out var localStatus);
            if (string.Equals(status, "PAID", StringComparison.OrdinalIgnoreCase))
            {
                localStatus = _store[orderCode] = "SUCCESS";
                var result = await _theTraTruocService.NapTien(IdThe, SoTien.Value, GhiChu, Convert.ToString( orderCode));
                if (result)
                {
                    ViewBag.Message = $"Đã nạp {SoTien:N0} đ vào thẻ {IdThe}.";
                }
                else
                {
                    ViewBag.Error = "Không thể nạp tiền. Thẻ có thể không tồn tại hoặc đã xảy ra lỗi.";
                }
            }

            if (localStatus != "SUCCESS")
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var info = await _payOS.getPaymentLinkInformation(orderCode);
                        if (string.Equals(info.status, "PAID", StringComparison.OrdinalIgnoreCase))
                        {
                            localStatus = _store[orderCode] = "SUCCESS";
                            break;
                        }
                    }
                    catch { /* bỏ qua và thử lại */ }
                    await Task.Delay(700);
                }
            }

            return View(new { orderCode, status = localStatus ?? "UNKNOWN" });
        }

        [HttpGet]
        public IActionResult Cancel()
        {
            return View();
        }

        // POST: /Payment/Create
        [HttpPost]
        public async Task<IActionResult> Create(string idThe, int soTien, string ghiChu)
        {
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var items = new List<ItemData>
            {
                new("Thanh toán thẻ trả trước", 1, soTien)
            };

            //string returnUrl = $"{_config["PayOS:ReturnUrl"]}?orderCode={orderCode}";
            string returnUrl = $"{_config["PayOS:ReturnUrl"]}?orderCode={orderCode}&IdThe={idThe}&SoTien={soTien}&GhiChu={Uri.EscapeDataString(ghiChu)}";

            string cancelUrl = $"{_config["PayOS:CancelUrl"]}?orderCode={orderCode}";

            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: soTien * 1,
                description: $"{ghiChu}",
                items: items,
                cancelUrl: cancelUrl,
                returnUrl: returnUrl
            );

            var result = await _payOS.createPaymentLink(paymentData);

            _store[orderCode] = "PENDING";
            // Chuyển hướng sang trang thanh toán PayOS
            return Redirect(result.checkoutUrl);
        }
    }
}
