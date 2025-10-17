using DAL;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly Chat_DAL _chatDAL = new Chat_DAL();

        // Trang chat box
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gửi tin nhắn từ khách hàng
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> SendMessage(int? sessionId, string message)
        {
            var kh = Session["Login"] as KhachHang;
            if (kh == null)
            {
                return Json(new { success = false, requiresLogin = true, loginUrl = Url.Action("Login", "Account") });
            }

            if (sessionId == null)
            {
                return Json(new { success = false, botReply = "Phiên chat không hợp lệ. Vui lòng tải lại trang!" });
            }

            string botReply;
            List<Product> products = new List<Product>();

            try
            {
                // 1. Lưu tin nhắn khách
                _chatDAL.InsertMessage(new ChatMessage
                {
                    SessionId = sessionId.Value,
                    SenderType = "Khách",
                    SenderId = kh.Id,
                    Message = message
                });

                // 2. Phân tích intent và tạo response thông minh
                var analysisResult = await AnalyzeMessageIntent(message);

                // Kết hợp các từ khóa sản phẩm và thuộc tính để tìm kiếm
                var searchKeywords = analysisResult.ProductKeywords.Concat(analysisResult.Attributes).ToList();
                string combinedKeywords = string.Join(" ", searchKeywords);

                if (analysisResult.Intent == "product_search" && searchKeywords.Any())
                {
                    // Tìm kiếm sản phẩm dựa trên các từ khóa đã được AI bóc tách
                    products = _chatDAL.ProductsByKeywords(searchKeywords);
                    botReply = await CreateProductRecommendationResponse(combinedKeywords, products, message);
                }
                else if (analysisResult.Intent == "health_consultation")
                {
                    botReply = await CreateHealthConsultationResponse(message, analysisResult.ExtractedInfo);
                }
                else if (analysisResult.Intent == "general_info")
                {
                    botReply = await CreateGeneralInfoResponse(message);
                }
                else
                {
                    botReply = await CreateGeneralChatResponse(message);
                }

                // 3. Lưu tin nhắn bot vào DB
                _chatDAL.InsertMessage(new ChatMessage
                {
                    SessionId = sessionId.Value,
                    SenderType = "Bot",
                    SenderId = null,
                    Message = botReply
                });

                // 4. Format sản phẩm cho frontend
                var formattedProducts = products.Select(p => new
                {
                    p.ThuocId,
                    ProductId = p.ThuocId,
                    p.TenThuoc,
                    Image = !string.IsNullOrEmpty(p.HinhAnh) ? p.HinhAnh : "/Content/assets/images/no-image.png",
                    p.GiaBan
                    //Description = p.m ?? ""
                }).ToList();

                return Json(new
                {
                    success = true,
                    botReply,
                    products = formattedProducts
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SendMessage Error: {ex.Message}");
                botReply = "Xin lỗi, tôi đang gặp chút vấn đề kỹ thuật. Bạn có thể liên hệ hotline 0979.310.351 để được hỗ trợ trực tiếp nhé!";
                return Json(new { success = false, botReply, products = new List<Product>() });
            }
        }

        /// <summary>
        /// Phân tích ý định của tin nhắn
        /// </summary>
        private async Task<MessageAnalysis> AnalyzeMessageIntent(string message)
        {
            // FIX: Dọn dẹp lại toàn bộ prompt cho gọn gàng và đúng cú pháp.
            string prompt = $@"
Bạn là AI trợ lý chuyên phân tích tin nhắn cho nhà thuốc Thanh Tứ.
Nhiệm vụ của bạn là đọc tin nhắn khách hàng và trích xuất thông tin dưới dạng JSON.

Tin nhắn của khách hàng:
'{message}'

Trả về JSON như sau:
{{
  ""intent"": ""product_search|health_consultation|general_info|greeting|other"",
  ""product_keywords"": [""tên thuốc hoặc thực phẩm chức năng""],
  ""attributes"": [""yêu cầu bổ sung, công dụng, thành phần, đối tượng sử dụng""],
  ""symptoms"": [""triệu chứng hoặc tình trạng sức khỏe khách đề cập""],
  ""extracted_info"": ""Tóm tắt nội dung quan trọng""
}}

---
QUY TẮC PHÂN TÍCH:
1. **intent**:
   - `product_search`: khi khách hỏi mua hoặc tìm loại thuốc cụ thể (Panadol, thuốc ho, vitamin C,...)
   - `health_consultation`: khi khách mô tả triệu chứng (đau đầu, ho, cảm, mất ngủ, tiêu hóa kém...)
   - `general_info`: hỏi kiến thức (thuốc này dùng sao, có tác dụng gì, uống khi nào)
   - `greeting`: chào hỏi
   - `other`: không liên quan đến thuốc

2. **product_keywords**: tên thuốc, vitamin, thực phẩm chức năng, nhóm thuốc.
3. **attributes**: công dụng, đặc tính (giảm đau, hạ sốt, tăng đề kháng, cho trẻ em, không gây buồn ngủ...).
4. **symptoms**: triệu chứng hoặc bệnh lý khách nêu ra.

---
VÍ DỤ:
- 'Mình cần mua Panadol Extra' → intent=product_search, product_keywords=['Panadol Extra']
- 'Bé nhà mình bị ho có đờm, nên dùng thuốc gì?' → intent=health_consultation, symptoms=['ho có đờm']
- 'Vitamin C uống buổi nào tốt nhất?' → intent=general_info, product_keywords=['Vitamin C']
";

            try
            {
                string aiResponse = await GetGeminiResponse(prompt);
                aiResponse = Regex.Replace(aiResponse, "```json", "", RegexOptions.IgnoreCase);
                aiResponse = Regex.Replace(aiResponse, "```", "", RegexOptions.IgnoreCase);
                aiResponse = aiResponse.Trim();
                var result = JsonConvert.DeserializeObject<MessageAnalysis>(aiResponse);

                if (result != null)
                {
                    if (result.ProductKeywords == null)
                    {
                        result.ProductKeywords = new List<string>();
                    }
                    if (result.Attributes == null)
                    {
                        result.Attributes = new List<string>();
                    }
                    if (result.Symptoms == null)
                    {
                        result.Symptoms = new List<string>();
                    }
                    return result;
                }
                return new MessageAnalysis { Intent = "other" };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AI Analysis error: {ex.Message}");
                // Fallback logic
                return new MessageAnalysis
                {
                    Intent = "product_search",
                    ProductKeywords = new List<string> { message } // Gửi toàn bộ tin nhắn làm từ khóa
                };
            }
        }

        /// <summary>
        /// Tạo phản hồi gợi ý sản phẩm
        /// </summary>
        private async Task<string> CreateProductRecommendationResponse(string keywords, List<Product> products, string originalMessage)
        {
            if (!products.Any())
            {
                return $"Rất tiếc, hiện tại chưa tìm thấy thuốc phù hợp cho <b>\"{keywords}\"</b>.<br>" +
                       "Bạn có thể mô tả rõ hơn triệu chứng hoặc liên hệ hotline <b>0979.310.351</b> để dược sĩ hỗ trợ nhé!";
            }

            var topProducts = products.Take(3).ToList();
            string productDetails = string.Join("\n", topProducts.Select(p =>
                $"- Tên: {p.TenThuoc}, Giá: {p.GiaBan:N0}đ"));

            string prompt = $@"
Bạn là dược sĩ online của nhà thuốc Thanh Tứ.
Khách vừa hỏi: '{originalMessage}'

Dựa vào đó, hệ thống đã tìm thấy các sản phẩm sau:
{productDetails}

Viết một phản hồi tư vấn (khoảng 40–60 từ):
1. Xác nhận lại nhu cầu (ví dụ: 'Bạn đang tìm {keywords} đúng không ạ?')
2. Gợi ý 1–2 thuốc nổi bật và công dụng ngắn gọn.
3. Kết thúc bằng câu hỏi mở: 'Bạn muốn mình tư vấn thêm về cách dùng không ạ?'
";
            try
            {
                return await GetGeminiResponse(prompt);
            }
            catch
            {
                // Fallback về cách trả lời cũ nếu AI lỗi
                string productInfo = string.Join("<br>", topProducts.Select(p =>
                    $"• <b>{p.TenThuoc}</b> - {p.GiaBan:N0}đ"));
                return $"Dạ, với nhu cầu tìm <b>\"{keywords}\"</b>, nhà thuốc Thanh Tứ gợi ý một số sản phẩm sau:<br>{productInfo}<br><br>" +
                       "Bạn muốn mình tư vấn chi tiết hơn về cách dùng không ạ?";
            }
        }

        /// <summary>
        /// Tạo phản hồi cho tư vấn chăm sóc da
        /// </summary>
        private async Task<string> CreateHealthConsultationResponse(string originalMessage, string extractedInfo)
        {
            string prompt = $@"Bạn là dược sĩ tư vấn của nhà thuốc Thanh Tứ. Khách nói: '{originalMessage}'

Nhiệm vụ:
1. Thể hiện sự quan tâm, đồng cảm.
2. Hỏi thêm 1–2 câu để hiểu rõ hơn tình trạng (triệu chứng kéo dài bao lâu, có dùng thuốc nào chưa,...).
3. Trả lời ngắn gọn (dưới 50 từ), thân thiện.
";
            try
            {
                return await GetGeminiResponse(prompt);
            }
            catch
            {
                return "Cảm ơn bạn đã chia sẻ. Bạn có thể nói rõ hơn về triệu chứng hoặc thời gian bị để mình hỗ trợ tư vấn thuốc phù hợp nhé!";
            }
        }

        /// <summary>
        /// Tạo phản hồi thông tin chung
        /// </summary>
        private async Task<string> CreateGeneralInfoResponse(string message)
        {
            string prompt = $@"
Bạn là dược sĩ online của nhà thuốc Thanh Tứ.
Khách hỏi: '{message}'

Trả lời:
- Ngắn gọn, dễ hiểu (30–50 từ).
- Thân thiện, chuyên nghiệp.
- Có thể gợi mở thêm về việc tìm mua thuốc liên quan.
";
            try
            {
                return await GetGeminiResponse(prompt);
            }
            catch
            {
                return "Cảm ơn câu hỏi của bạn! Để được tư vấn chi tiết nhất, bạn có thể liên hệ hotline <b>0979.310.351</b> nhé!";
            }
        }

        /// <summary>
        /// Tạo phản hồi chat chung
        /// </summary>
        private async Task<string> CreateGeneralChatResponse(string message)
        {
            string lowerMessage = message.ToLower();

            if (lowerMessage.Contains("xin chào") || lowerMessage.Contains("hello") || lowerMessage.Contains("hi"))
            {
                return "Chào bạn 👋 Tôi là dược sĩ online của nhà thuốc Thanh Tứ. Tôi có thể giúp bạn tìm thuốc, tư vấn sức khỏe hoặc giải đáp cách dùng thuốc. Bạn cần hỗ trợ gì ạ?";
            }

            if (lowerMessage.Contains("cảm ơn") || lowerMessage.Contains("thank"))
            {
                return "Rất vui được hỗ trợ bạn! Nếu còn thắc mắc về thuốc, hãy nhắn cho mình bất cứ lúc nào nhé 💊";
            }

            string prompt = $@"
Bạn là dược sĩ tư vấn online của Hoa Pharmacy.
Khách nói: '{message}'

Trả lời ngắn (dưới 50 từ), thân thiện.
Nếu tin nhắn không liên quan thuốc, hãy khéo léo chuyển hướng về sức khỏe hoặc dược phẩm.
";
            try
            {
                return await GetGeminiResponse(prompt);
            }
            catch
            {
                return "Cảm ơn bạn! Nếu bạn muốn tư vấn về thuốc hoặc sức khỏe, mình luôn sẵn sàng hỗ trợ nhé 💊";
            }
        }

        [HttpGet]
        public ActionResult GetMessages(int sessionId)
        {
            try
            {
                var messages = _chatDAL.GetMessagesBySession(sessionId);
                return Json(messages ?? new List<ChatMessage>(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetMessages error: {ex.Message}");
                return Json(new List<ChatMessage>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult StartSession(int? customerId)
        {
            try
            {
                int newSessionId = _chatDAL.CreateSession(customerId);
                return Json(new { success = true, sessionId = newSessionId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StartSession error: {ex.Message}");
                return Json(new { success = false, error = "Không thể tạo session chat" });
            }
        }

        private async Task<string> GetGeminiResponse(string userMessage)
        {
            string apiKey = ConfigurationManager.AppSettings["Gemini_API_Key"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Gemini API key not configured");
            }
            string endpoint = "https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key=" + apiKey;



            var request = new
            {
                contents = new[]
                {
            new { parts = new[] { new { text = userMessage } } }
        },
                // Thêm cài đặt an toàn để tránh bị chặn bởi các bộ lọc mặc định
                safetySettings = new[]
                {
            new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_NONE" },
            new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_NONE" },
            new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_NONE" },
            new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_NONE" }
        }
            };

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                var jsonRequest = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Ghi lại (log) yêu cầu gửi đi để debug
                System.Diagnostics.Debug.WriteLine($"Gemini Request: {jsonRequest}");

                var response = await client.PostAsync(endpoint, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Ghi lại (log) toàn bộ câu trả lời từ Google để debug
                System.Diagnostics.Debug.WriteLine($"Gemini Response: {jsonResponse}");

                if (!response.IsSuccessStatusCode)
                {
                    // Lỗi này sẽ bao gồm thông tin chi tiết hơn
                    throw new Exception($"Gemini API error: {response.StatusCode} - {jsonResponse}");
                }

                dynamic geminiRes = JsonConvert.DeserializeObject(jsonResponse);

                // KIỂM TRA PHẢN HỒI AN TOÀN VÀ TRỐNG
                if (geminiRes?.promptFeedback?.blockReason != null)
                {
                    string reason = geminiRes.promptFeedback.blockReason;
                    System.Diagnostics.Debug.WriteLine($"Request blocked due to safety settings. Reason: {reason}");
                    return "Rất tiếc, yêu cầu của bạn có chứa nội dung nhạy cảm và đã bị chặn. Vui lòng thử lại với một câu hỏi khác.";
                }

                if (geminiRes?.candidates == null || geminiRes.candidates.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Gemini response is valid but contains no candidates.");
                    // Trả về một câu trả lời thân thiện thay vì gây ra lỗi
                    return "Xin lỗi, tôi chưa thể xử lý yêu cầu này. Bạn có thể diễn đạt lại câu hỏi được không?";
                }

                string responseText = geminiRes?.candidates?[0]?.content?.parts?[0]?.text;

                if (string.IsNullOrEmpty(responseText))
                {
                    System.Diagnostics.Debug.WriteLine("Gemini response is valid but the text is empty.");
                    return "Xin lỗi, tôi chưa thể xử lý yêu cầu này. Bạn có thể diễn đạt lại câu hỏi được không?";
                }

                return responseText;
            }
        }
    }

    public class MessageAnalysis
    {
        [JsonProperty("intent")]
        public string Intent { get; set; } = "other";

        [JsonProperty("product_keywords")]
        public List<string> ProductKeywords { get; set; } = new List<string>();

        [JsonProperty("attributes")]
        public List<string> Attributes { get; set; } = new List<string>();

        [JsonProperty("symptoms")]
        public List<string> Symptoms { get; set; } = new List<string>();

        [JsonProperty("extracted_info")]
        public string ExtractedInfo { get; set; }
    }
}