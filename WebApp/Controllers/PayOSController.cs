using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;

namespace PayOSDemo.Controllers
{
    [Route("payos")]
    public class PayOSController : Controller
    {
        private readonly PayOS _payOS;
        private readonly IConfiguration _config;
        private readonly IDictionary<long, string> _store;

        public PayOSController(PayOS payOS, IConfiguration config, IDictionary<long, string> store)
        {
            _payOS = payOS;
            _config = config;
            _store = store;
        }

        // POST /payos/create-link
        [HttpPost("create-link")]
        public async Task<IActionResult> CreateLink([FromBody] CreateOrderRequest body)
        {
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var items = new List<ItemData>
            {
                new ItemData(body.ItemName ?? "San pham", body.Quantity, body.Price)
            };

            // GẮN orderCode vào Return/Cancel để khi user quay về ta biết đơn nào
            string baseReturn = _config["PayOS:ReturnUrl"] ?? "";
            string baseCancel = _config["PayOS:CancelUrl"] ?? "";
            string returnUrl = AppendQuery(baseReturn, $"orderCode={orderCode}");
            string cancelUrl = AppendQuery(baseCancel, $"orderCode={orderCode}");

            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: body.Price * body.Quantity,
                description: $"Thanh toan don hang #{orderCode}",
                items: items,
                cancelUrl: cancelUrl,
                returnUrl: returnUrl
            );

            var result = await _payOS.createPaymentLink(paymentData);

            _store[orderCode] = "PENDING";

            return Ok(new
            {
                orderCode = result.orderCode,
                status = result.status,
                checkoutUrl = result.checkoutUrl,
                qrCode = result.qrCode
            });
        }

        // POST /payos/confirm-webhook  (chạy 1 lần sau khi set PayOS:WebhookUrl)
        [HttpPost("confirm-webhook")]
        public async Task<IActionResult> ConfirmWebhook()
        {
            var url = _config["PayOS:WebhookUrl"];
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Thiếu PayOS:WebhookUrl trong cấu hình.");

            var res = await _payOS.confirmWebhook(url);
            return Ok(new { ok = true, message = "Webhook confirmed", res });
        }

        // POST /payos/webhook  (PayOS gọi về khi có cập nhật)
        [HttpPost("webhook")]
        public IActionResult Webhook([FromBody] WebhookType payload)
        {
            WebhookData data;
            try
            {
                data = _payOS.verifyPaymentWebhookData(payload); // verify chữ ký + parse
            }
            catch (Exception ex)
            {
                return BadRequest(new { ok = false, error = ex.Message });
            }

            // Idempotent
            if (_store.TryGetValue(data.orderCode, out var current) && current == "SUCCESS")
                return Ok(new { ok = true, message = "Already processed" });

            if (data.code == "00")
            {
                _store[data.orderCode] = "SUCCESS";
            }
            else
            {
                _store[data.orderCode] = "FAILED";
            }

            return Ok(new { ok = true });
        }

        // GET /payos/return?orderCode=...
        // Nếu webhook chưa về kịp -> chủ động hỏi PayOS để chắc chắn biết đã thanh toán
        [HttpGet("return")]
        public async Task<IActionResult> Return([FromQuery] long orderCode)
        {
            _store.TryGetValue(orderCode, out var status);

            if (status != "SUCCESS")
            {
                // Thử hỏi PayOS 3 lần (tối đa ~3s) để đợi webhook/propgation
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var info = await _payOS.getPaymentLinkInformation(orderCode);
                        // Tùy SDK, thường info.status == "PAID" là thành công
                        if (string.Equals(info.status, "PAID", StringComparison.OrdinalIgnoreCase))
                        {
                            status = _store[orderCode] = "SUCCESS";
                            break;
                        }
                    }
                    catch
                    {
                        // bỏ qua, thử lại
                    }
                    await Task.Delay(1000);
                }
            }
            return View("~/Views/Payment/Return.cshtml", new { orderCode, status });
        }

        [HttpGet("cancel")]
        public IActionResult Cancel([FromQuery] long? orderCode)
        {
            return View("~/Views/Payment/Cancel.cshtml");
        }

        [HttpGet("status/{orderCode:long}")]
        public IActionResult Status(long orderCode)
        {
            _store.TryGetValue(orderCode, out var status);
            return Ok(new { orderCode, status = status ?? "UNKNOWN" });
        }

        [HttpGet("status-view/{orderCode:long}")]
        public IActionResult StatusView(long orderCode)
        {
            _store.TryGetValue(orderCode, out var status);
            return View("~/Views/Payment/Status.cshtml", new { orderCode, status = status ?? "UNKNOWN" });
        }

        // DTO tạo đơn
        public record CreateOrderRequest(string? ItemName, int Quantity, int Price);

        // Helper: nối query vào URL (giữ lại query cũ nếu có)
        private static string AppendQuery(string? baseUrl, string extraQuery)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) return "";
            return baseUrl.Contains('?') ? $"{baseUrl}&{extraQuery}" : $"{baseUrl}?{extraQuery}";
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}
