using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class ChatAdminController : Controller
    {
        private Chat_DAL _chatDAL = new Chat_DAL();

        // Danh sách các phiên chat
        public ActionResult Index()
        {
            var sessions = _chatDAL.GetAllSessions();
            return View(sessions);
        }

        // Xem chi tiết chat trong 1 session
        public ActionResult Chat(int sessionId)
        {
            ViewBag.SessionId = sessionId;
            return View();
        }

        [HttpPost]
        public ActionResult SendMessage(int sessionId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { success = false, error = "Tin nhắn không được để trống!" });

            var adminMsg = new ChatMessage
            {
                SessionId = sessionId,
                SenderType = "Admin",
                SenderId = 1, // nếu có hệ thống phân quyền admin thì lấy từ user login
                Message = message
            };
            _chatDAL.InsertMessage(adminMsg);

            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult GetMessages(int sessionId)
        {
            var messages = _chatDAL.GetMessagesBySession(sessionId);
            return Json(messages, JsonRequestBehavior.AllowGet);
        }
    }
}