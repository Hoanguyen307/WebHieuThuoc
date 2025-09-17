using DAL;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class ChatController : Controller
    {
        private Chat_DAL _chatDAL = new Chat_DAL();

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

                if (analysisResult.Intent == "product_search")
                {
                    // Tìm kiếm sản phẩm
                    products = _chatDAL.ProductsByKeyword(analysisResult.Keywords);
                    botReply = CreateProductRecommendationResponse(analysisResult.Keywords, products, message);
                }
                else if (analysisResult.Intent == "skincare_consultation")
                {
                    // Tư vấn chăm sóc da
                    botReply = await CreateSkincareConsultationResponse(message, analysisResult.ExtractedInfo);
                }
                else if (analysisResult.Intent == "general_info")
                {
                    // Thông tin chung về mỹ phẩm
                    botReply = await CreateGeneralInfoResponse(message);
                }
                else
                {
                    // Trả lời chung
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

                // 4. Format sản phẩm cho frontend với đầy đủ thông tin
                var formattedProducts = products.Select(p => new
                {
                    Id = p.ThuocId,
                    ProductId = p.ThuocId,
                    Name = p.TenThuoc,
                    Image = !string.IsNullOrEmpty(p.HinhAnh) ? p.HinhAnh : "/Content/assets/images/no-image.png",
                    Price = p.GiaGoc,
                    Description = p.QuyCach ?? ""
                }).ToList();

                return Json(new
                {
                    success = true,
                    botReply = botReply,
                    products = formattedProducts
                });
            }
            catch (Exception )
            {
                botReply = "Xin lỗi, tôi đang gặp chút vấn đề kỹ thuật. Bạn có thể liên hệ hotline 0979.310.351 để được hỗ trợ trực tiếp nhé!";
            }

            // 4. Trả về JSON
            return Json(new { success = true, botReply, products = new List<Product>() });
        }

        /// <summary>
        /// Phân tích ý định của tin nhắn
        /// </summary>
        private async Task<MessageAnalysis> AnalyzeMessageIntent(string message)
        {
            string lowerMessage = message.ToLower();
            string prompt = $@"
Phân tích tin nhắn của khách hàng về mỹ phẩm sau:
'{message}'

Trả về JSON với format:
{{
    ""intent"": ""product_search|skincare_consultation|general_info|greeting|other"",
    ""keywords"": ""từ khóa chính để tìm sản phẩm"",
    ""skin_type"": ""da dầu|da khô|da hỗn hợp|da nhạy cảm|không xác định"",
    ""skin_concerns"": [""mụn"", ""thâm"", ""lão hóa"", ""tàn nhang"", ""v.v""],
    ""age_range"": ""teens|20s|30s|40s|50+|không xác định"",
    ""extracted_info"": ""thông tin bổ sung quan trọng""
}}
Quy tắc phân loại intent:
- product_search: khi khách hàng TÌM KIẾM sản phẩm cụ thể (kem chống nắng, serum, son môi...)
- skincare_consultation: khi khách hàng HỎI VỀ VẤN ĐỀ DA (mụn, thâm, da khô...) hoặc xin tư vấn routine
- general_info: khi hỏi về thành phần, cách dùng, tips chăm sóc da chung
- greeting: lời chào (xin chào, hello, hi...)
- other: các tin nhắn khác

Ví dụ phân tích:
- 'Da mình hay bị mụn' → intent: skincare_consultation, skin_concerns: [""mụn""]
- 'Tìm kem chống nắng' → intent: product_search, keywords: ""kem chống nắng""
- 'Xin chào' → intent: greeting
";
            string[] productKeywords = { "kem", "serum", "son", "phấn", "sữa rửa mặt", "toner", "nước hoa" };

            if (productKeywords.Any(k => lowerMessage.Contains(k)) ||
                lowerMessage.Contains("tìm") ||
                lowerMessage.Contains("mua"))
            {
                return new MessageAnalysis
                {
                    Intent = "product_search",
                    Keywords = ExtractSimpleKeywords(message)
                };
            }
            try
            {
                string aiResponse = await GetGeminiResponse(prompt);
                // Loại bỏ markdown formatting nếu có
                aiResponse = aiResponse.Replace("```json", "").Replace("```", "").Trim();

                var result = JsonConvert.DeserializeObject<MessageAnalysis>(aiResponse);
                return result ?? new MessageAnalysis { Intent = "other", Keywords = ExtractSimpleKeywords(message) };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AI Analysis error: {ex.Message}");

                // Fallback logic dựa trên từ khóa đơn giản

                if (lowerMessage.Contains("tìm") || lowerMessage.Contains("mua") ||
                    lowerMessage.Contains("kem") || lowerMessage.Contains("serum") ||
                    lowerMessage.Contains("son") || lowerMessage.Contains("phấn") ||
                    lowerMessage.Contains("sữa rửa mặt") || lowerMessage.Contains("toner") ||
                    lowerMessage.Contains("nước hoa"))
                {
                    return new MessageAnalysis
                    {
                        Intent = "product_search",
                        Keywords = ExtractSimpleKeywords(message)
                    };
                }
                else if (lowerMessage.Contains("da") || lowerMessage.Contains("mụn") ||
                         lowerMessage.Contains("tư vấn") || lowerMessage.Contains("routine"))
                {
                    return new MessageAnalysis
                    {
                        Intent = "skincare_consultation",
                        Keywords = ExtractSimpleKeywords(message)
                    };
                }
                else
                {
                    string extracted = ExtractSimpleKeywords(message);
                    if (extracted.Contains("nước hoa"))
                    {
                        return new MessageAnalysis
                        {
                            Intent = "product_search",
                            Keywords = extracted
                        };
                    }

                    return new MessageAnalysis
                    {
                        Intent = "other",
                        Keywords = extracted
                    };
                }

            }
        }

        /// <summary>
        /// Tạo phản hồi cho tư vấn chăm sóc da
        /// </summary>
        private async Task<string> CreateSkincareConsultationResponse(string originalMessage, string extractedInfo)
        {
            string prompt = $@"
                Bạn là chuyên gia tư vấn mỹ phẩm của Hoa Cosmetics. Khách hàng hỏi: '{originalMessage}'

                Nhiệm vụ:
                    - Trả lời ngắn gọn (30-40 từ).
                    - Tập trung đặt câu hỏi để hiểu rõ hơn về loại da (khô, dầu, hỗn hợp, nhạy cảm) và vấn đề da (mụn, nám, lão hoá…).
                    - Không cần đưa routine sẵn.
                    - Giữ giọng văn thân thiện, khuyến khích khách hàng chia sẻ thêm.
            ";

            try
            {
                return await GetGeminiResponse(prompt);
            }
            catch
            {
                return "Cảm ơn bạn đã chia sẻ về tình trạng da. Để tư vấn chính xác nhất, bạn có thể liên hệ hotline <b>0979.310.351</b> để được chuyên gia hỗ trợ trực tiếp nhé!";
            }
        }

        /// <summary>
        /// Tạo phản hồi gợi ý sản phẩm
        /// </summary>
        private string CreateProductRecommendationResponse(string keywords, List<Product> products, string originalMessage)
        {
            if (products.Any())
            {
                var topProducts = products.Take(3).ToList();
                string productInfo = string.Join("<br>", topProducts.Select(p =>
                    $"• <b>{p.TenThuoc}</b> - {p.GiaGoc:N0}đ"));

                return $"Bạn đang quan tâm đến <b>\"{keywords}\"</b>?<br>" +
               $"Mình gợi ý một vài sản phẩm tiêu biểu:<br>{productInfo}<br><br>" +
               "Bạn muốn mình tư vấn chi tiết hơn về nhu cầu (ví dụ: dịp sử dụng, phong cách mùi hương ưa thích) không?";
            }
            else
            {
                return $"Hiện tại chưa tìm thấy sản phẩm phù hợp cho <b>\"{keywords}\"</b>.<br>" +
               "Bạn có thể cho mình biết rõ hơn nhu cầu hoặc liên hệ hotline <b>0979.310.351</b> để được tư vấn nhanh nhé!";
            }
        }

        /// <summary>
        /// Tạo phản hồi thông tin chung
        /// </summary>
        private async Task<string> CreateGeneralInfoResponse(string message)
        {
            string prompt = $@"
                Khách hàng hỏi: '{message}'

                Nhiệm vụ:
                    - Trả lời ngắn gọn (30-50 từ)
                    - Nếu cần, đặt câu hỏi để hiểu rõ nhu cầu của khách
                    - Giữ phong cách thân thiện, chuyên nghiệp, dễ hiểu
                    - Có thể dùng HTML đơn giản như <br>, <b>, <i>
            ";

            try
            {
                return await GetGeminiResponse(prompt);
            }
            catch
            {
                return "Cảm ơn câu hỏi của bạn! Để được tư vấn chi tiết nhất về sản phẩm này, bạn có thể liên hệ hotline <b>0979.310.351</b> hoặc xem thêm thông tin tại website nhé!";
            }
        }

        /// <summary>
        /// Tạo phản hồi chat chung
        /// </summary>
        private async Task<string> CreateGeneralChatResponse(string message)
        {
            string lowerMessage = message.ToLower();

            // Các câu trả lời cố định cho một số trường hợp phổ biến
            if (lowerMessage.Contains("xin chào") || lowerMessage.Contains("hello") ||
                lowerMessage.Contains("hi") || lowerMessage.Contains("chào"))
            {
                return "Chào bạn! Tôi là trợ lý tư vấn mỹ phẩm của Hoa Cosmetics. Tôi có thể giúp bạn:<br>" +
                       "• <b>Tìm kiếm sản phẩm</b> phù hợp<br>" +
                       "• <b>Tư vấn chăm sóc da</b><br>" +
                       "• <b>Giải đáp thắc mắc</b> về mỹ phẩm<br>" +
                       "• <b>Thông tin khuyến mãi</b> mới nhất<br><br>" +
                       "Bạn cần hỗ trợ gì ạ?";
            }

            if (lowerMessage.Contains("cảm ơn") || lowerMessage.Contains("thank") ||
                lowerMessage.Contains("cám ơn"))
            {
                return "Rất vui được hỗ trợ bạn!<br>Nếu còn thắc mắc gì khác về mỹ phẩm, đừng ngại nhắn tin cho tôi nhé!<br><br>" +
                       "Hotline hỗ trợ: <b>0979.310.351</b>";
            }

            if (lowerMessage.Contains("giá") || lowerMessage.Contains("khuyến mãi") ||
                lowerMessage.Contains("giảm giá") || lowerMessage.Contains("ưu đãi"))
            {
                return "Hoa Cosmetics thường xuyên có các chương trình khuyến mãi hấp dẫn!<br><br>" +
                       "• <b>Freeship</b> cho đơn từ 15K<br>" +
                       "• <b>Quà tặng</b> cho đơn từ 499K<br>" +
                       "• <b>Giao hàng nhanh</b> trong 24h<br>" +
                       "• <b>Cam kết hàng chính hãng</b> 100%<br><br>" +
                       "Xem chi tiết tại <a href='/' target='_blank'>website</a> hoặc gọi hotline <b>0979.310.351</b>!";
            }

            if (lowerMessage.Contains("hotline") || lowerMessage.Contains("liên hệ") ||
                lowerMessage.Contains("địa chỉ"))
            {
                return "<b>Thông tin liên hệ Hoa Cosmetics:</b><br><br>" +
                       "Hotline: <b>0979.310.351</b><br>" +
                       "Chat trực tuyến: <b>24/7</b><br>" +
                       "Website: <a href='/' target='_blank'>hoacosmetics.vn</a><br>" +
                       "Địa chỉ: [Địa chỉ cửa hàng]<br>" +
                       "Giao hàng: <b>Toàn quốc 24h</b>";
            }

            // Sử dụng AI cho các câu hỏi khác
            string prompt = $@"
Bạn là trợ lý tư vấn mỹ phẩm thân thiện của Hoa Cosmetics.
Khách hàng nói: '{message}'

Trả lời ngắn gọn (50-80 từ), thân thiện và hữu ích.
Nếu không liên quan đến mỹ phẩm, hãy lịch sự chuyển hướng về chủ đề chăm sóc da/làm đẹp.
Có thể dùng HTML đơn giản: <br>, <b>, <i>
Không sử dụng emoji.
";

            try
            {
                return await GetGeminiResponse(prompt);
            }
            catch
            {
                return "Cảm ơn bạn đã nhắn tin!<br><br>Để được hỗ trợ tốt nhất về mỹ phẩm, bạn có thể:<br>" +
                       "• Liên hệ hotline <b>0979.310.351</b><br>" +
                       "• Hỏi tôi về các sản phẩm chăm sóc da<br>" +
                       "• Xem <a href='/Product' target='_blank'>sản phẩm mới nhất</a>";
            }
        }

        /// <summary>
        /// Trích xuất từ khóa đơn giản khi AI không hoạt động
        /// </summary>
        private string ExtractSimpleKeywords(string message)
        {
            var keywords = new Dictionary<string, string>
            {
                {"kem chống nắng", "kem chống nắng"},
                {"serum", "serum"},
                {"mặt nạ", "mặt nạ"},
                {"sữa rửa mặt", "sữa rửa mặt"},
                {"toner", "toner"},
                {"kem dưỡng", "kem dưỡng"},
                {"son môi", "son môi"},
                {"phấn nền", "phấn nền"},
                {"mascara", "mascara"},
                {"eyeliner", "eyeliner"},
                {"cushion", "cushion"},
                {"mụn", "trị mụn"},
                {"thâm", "trị thâm"},
                {"lão hóa", "chống lão hóa"},
                {"dưỡng ẩm", "dưỡng ẩm"},
                {"làm trắng", "làm trắng da"},
                {"se khít lỗ chân lông", "se khít lỗ chân lông"},
                {"nước hoa", "nước hoa"},
                {"nước hoa nam", "nước hoa nam"},
                {"nước hoa nữ", "nước hoa nữ"},

            };

            string lowerMessage = message.ToLower();
            foreach (var kvp in keywords)
            {
                if (lowerMessage.Contains(kvp.Key))
                    return kvp.Value;
            }

            return message;
        }

        /// <summary>
        /// Lấy toàn bộ tin nhắn trong session
        /// </summary>
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

        /// <summary>
        /// Khởi tạo session chat mới
        /// </summary>
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

            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            var request = new
            {
                contents = new[]
                {
                    new {
                        parts = new[] {
                            new { text = userMessage }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 1000,
                    topP = 0.8,
                    topK = 40
                }
            };

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Gemini API error: {response.StatusCode} - {result}");
                    throw new Exception($"Gemini API error: {response.StatusCode}");
                }

                dynamic geminiRes = JsonConvert.DeserializeObject(result);
                string responseText = geminiRes?.candidates?[0]?.content?.parts?[0]?.text;

                if (string.IsNullOrEmpty(responseText))
                {
                    throw new Exception("Empty response from Gemini");
                }

                return responseText;
            }
        }
    }

    // Model hỗ trợ phân tích tin nhắn
    public class MessageAnalysis
    {
        public string Intent { get; set; }
        public string Keywords { get; set; }
        public string SkinType { get; set; }
        public List<string> SkinConcerns { get; set; } = new List<string>();
        public string AgeRange { get; set; }
        public string ExtractedInfo { get; set; }
    }
}