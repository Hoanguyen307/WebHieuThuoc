
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Extension
{
    public static class myExtension
    {

        #region "Convert"
        // To - throw khi gặp exception
        public static string ToDateRFC3987(this object obj)
        {
            try
            {
                DateTime dt = obj.ToDate();
                if (dt == DateTime.MinValue)
                {
                    return "";
                }
                else
                {
                    String _mm = dt.Month > 9 ? dt.Month.MapStr() : ("0" + dt.Month.MapStr());
                    String _dd = dt.Day > 9 ? dt.Day.MapStr() : ("0" + dt.Day.MapStr());
                    return dt.Year + "-" + _mm + "-" + _dd;
                }
            }
            catch
            {
                return String.Empty;
            }
        }
        public static string ToDateVietNam(this object obj, bool ddMM = false)
        {
            try
            {
                DateTime dt = obj.ToDate();
                return dt == DateTime.MinValue ? "" : (ddMM ? dt.Date.ToString("dd/MM") : dt.Date.ToShortDateString());
            }
            catch
            {
                return String.Empty;
            }
        }
        public static string ToFullDateVietNam(this object obj)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(obj);
                return dt.Hour.ToString() + "h " + (dt.Minute >= 10 ? dt.Minute.ToString() : ("0" + dt.Minute.ToString())) + "' ngày " + dt.ToShortDateString();
            }
            catch
            {
                return String.Empty;
            }
        }
        public static string NgayThangNam(this DateTime obj)
        {
            String result = "";
            if (obj.Day < 10) { result = "ngày 0" + obj.Day.ToString(); } else { result = "ngày " + obj.Day.ToString(); }
            if (obj.Month < 10) { result += " tháng 0" + obj.Month.ToString(); } else { result += " tháng " + obj.Month.ToString(); }
            result += " năm " + obj.Year.ToString();
            return result;
        }
        public static int ToInt32(this object obj)
        {
            try
            {

                return Int32.Parse(obj.ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static Double ToDouble(this object obj)
        {
            try
            {
                return Double.Parse(obj.ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static float ToFloat(this object obj)
        {
            try
            {
                return float.Parse(obj.ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static DateTime ToDate(this object obj)
        {
            try
            {
                return DateTime.Parse(obj.ToString()).Date;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Boolean ToBoolean(this object obj)
        {
            try
            {
                return (obj.ToString().ToUpper().Trim().Equals("TRUE") || obj.ToString().Trim().Equals("1")) ? true : false;
            }
            catch (Exception)
            {

                throw;
            }

        }
        // Map - luôn trả về éo quan tâm ngoại lệ cl gì hết
        public static string KhongDau(this string chucodau) // trả về chuỗi không dấu, không khoảng trắng , lower
        {
            chucodau = chucodau.ToLower();
            string FindText = " áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            string ReplText = "_aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index = -1;
            char[] arrChar = FindText.ToCharArray();
            while ((index = chucodau.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(chucodau[index]);
                chucodau = chucodau.Replace(chucodau[index], ReplText[index2]);
            }
            return chucodau;
        }
        public static int MapInt(this object obj, bool fromMoney = false)
        {
            try
            {
                if (obj == null) return 0;
                int n;
                if (fromMoney)
                {
                    obj = obj.MapStr().Trim().Replace(",", "").Replace(".", "").Replace(" ", "");
                }
                bool isNumeric = int.TryParse(obj.ToString(), out n);
                if (isNumeric)
                {
                    return n;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
        public static string MapStr(this object obj, bool tienTe = false)
        {
            try
            {
                String result = "";
                if (obj != null) result = obj.ToString(); else return string.Empty;
                if (tienTe)
                {
                    result = string.Format("{0:0,0}", result.MapDouble());
                    result = result.Replace(",", ".");
                    //result = string.Format("{0:0,0}", "5225999994774".MapDouble());
                }
                return result;
            }
            catch
            {

                return String.Empty;
            }
        }
        public static Double MapDouble(this object obj, int lenght = 2)
        {
            try
            {
                if (obj == null) return 0;
                return Math.Round(obj.ToDouble(), lenght);
            }
            catch
            {

                return 0;
            }
        }
        public static DateTime MapDate(this object obj)
        {
            try
            {
                if (obj == null) return DateTime.MinValue;
                return obj.ToDate();
            }
            catch
            {

                return DateTime.MinValue;
            }
        }
        public static DateTime? MapDateNullAble(this object obj)
        {
            try
            {
                if (obj == null) return null;
                return obj.ToDate();
            }
            catch
            {

                return null;
            }
        }
        public static Boolean MapBool(this object obj)
        {
            try
            {
                if (obj == null) return false;
                return obj.ToBoolean();
            }
            catch
            {

                return false;
            }
        }
        public static float MapFloat(this object obj)
        {
            try
            {
                if (obj == null) return 0;
                return obj.ToFloat();
            }
            catch
            {
                return 0;
            }
        }

        #endregion
        #region "For DataTable"
        public static bool Contained(this DataTable dt, string columnName)
        {
            if (dt.Columns.Contains(columnName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void ClearCaption(this DataTable dt)
        {
            foreach (DataColumn item in dt.Columns)
            {
                item.Caption = String.Empty;
            }
        }
        public static void SetKeyCol(this DataTable dt, string colName)
        {
            try
            {
                if (dt != null)
                {
                    dt.Columns[colName.ToString()].Caption = "Key";
                }
            }
            catch
            {

            }
        }
        public static void ImportCol(this DataTable dt, DataTable _dataTable, string colName, string colCaption = "", bool tienTe = false)
        {
            if (tienTe)
            {
                colCaption += "($)";
            }
            if (_dataTable.Contained(colName))
            {
                int c = 0;
                foreach (DataColumn item in dt.Columns)
                {
                    if (item.ColumnName == colName)
                    {
                        c++;
                    }
                }
                if (c > 0) { colName = colName + "_" + c.ToString(); }
                DataColumn col = new DataColumn(colName);
                col.DataType = _dataTable.Columns[colName].DataType;
                dt.Columns.Add(col);
                String s = dt.Columns[colName].GetType().ToString();
                if (colCaption != "")
                {
                    dt.Columns[colName].Caption = colCaption;
                }
                else
                {
                    dt.Columns[colName].Caption = colName;
                }
                foreach (DataRow item in _dataTable.Rows)
                {
                    if (dt.Rows.Count < _dataTable.Rows.Count)
                    {
                        dt.ImportRow(item);
                    }
                    else
                    {
                        dt.Rows[_dataTable.Rows.IndexOf(item)][colName] = item[colName];
                    }

                }
            }
        }
        public static int HashCode(this Object obj)
        {
            if (obj != null)
            {
                return obj.GetHashCode();
            }
            return 0;
        }
        public static void ChangeCaptionCol(this DataTable dt, string colName, string Caption)
        {
            try
            {
                dt.Columns[colName.ToString()].Caption = Caption;
            }
            catch { }
        }
        public static void RemoveNoneCaptionCol(this DataTable dt)
        {
            DataTable copy = dt.Copy();
            int countDeletedCol = 0;
            foreach (DataColumn item in copy.Columns)
            {
                if (item.Caption.Equals(String.Empty))
                {
                    dt.Columns.RemoveAt(copy.Columns.IndexOf(item) - countDeletedCol);
                    countDeletedCol++;
                }
            }
        }
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        #endregion

        #region "Doc so thanh chu"
        private static string Chu(string gNumber)
        {
            string result = "";
            switch (gNumber)
            {
                case "0":
                    result = "không";
                    break;
                case "1":
                    result = "một";
                    break;
                case "2":
                    result = "hai";
                    break;
                case "3":
                    result = "ba";
                    break;
                case "4":
                    result = "bốn";
                    break;
                case "5":
                    result = "năm";
                    break;
                case "6":
                    result = "sáu";
                    break;
                case "7":
                    result = "bảy";
                    break;
                case "8":
                    result = "tám";
                    break;
                case "9":
                    result = "chín";
                    break;
            }
            return result;
        }
        private static string Tach(string tach3)
        {
            string Ktach = "";
            if (tach3.Equals("000"))
                return "";
            if (tach3.Length == 3)
            {
                string tr = tach3.Trim().Substring(0, 1).ToString().Trim();
                string ch = tach3.Trim().Substring(1, 1).ToString().Trim();
                string dv = tach3.Trim().Substring(2, 1).ToString().Trim();
                if (tr.Equals("0") && ch.Equals("0"))
                    Ktach = " không trăm lẻ " + Chu(dv.ToString().Trim()) + " ";
                if (!tr.Equals("0") && ch.Equals("0") && dv.Equals("0"))

                    Ktach = Chu(tr.ToString().Trim()).Trim() + " trăm ";
                if (!tr.Equals("0") && ch.Equals("0") && !dv.Equals("0"))
                    Ktach = Chu(tr.ToString().Trim()).Trim() + " trăm lẻ " + Chu(dv.Trim()).Trim() + " ";
                if (tr.Equals("0") && Convert.ToInt32(ch) > 1 && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = " không trăm " + Chu(ch.Trim()).Trim() + " mươi " + Chu(dv.Trim()).Trim() + " ";
                if (tr.Equals("0") && Convert.ToInt32(ch) > 1 && dv.Equals("0"))
                    Ktach = " không trăm " + Chu(ch.Trim()).Trim() + " mươi ";
                if (tr.Equals("0") && Convert.ToInt32(ch) > 1 && dv.Equals("5"))
                    Ktach = " không trăm " + Chu(ch.Trim()).Trim() + " mươi lăm ";
                if (tr.Equals("0") && ch.Equals("1") && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = " không trăm mười " + Chu(dv.Trim()).Trim() + " ";
                if (tr.Equals("0") && ch.Equals("1") && dv.Equals("0"))
                    Ktach = " không trăm mười ";
                if (tr.Equals("0") && ch.Equals("1") && dv.Equals("5"))
                    Ktach = " không trăm mười lăm ";
                if (Convert.ToInt32(tr) > 0 && Convert.ToInt32(ch) > 1 && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm " + Chu(ch.Trim()).Trim() + " mươi " + Chu(dv.Trim()).Trim() + " ";
                if (Convert.ToInt32(tr) > 0 && Convert.ToInt32(ch) > 1 && dv.Equals("0"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm " + Chu(ch.Trim()).Trim() + " mươi ";
                if (Convert.ToInt32(tr) > 0 && Convert.ToInt32(ch) > 1 && dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm " + Chu(ch.Trim()).Trim() + " mươi lăm ";
                if (Convert.ToInt32(tr) > 0 && ch.Equals("1") && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm mười " + Chu(dv.Trim()).Trim() + " ";
                if (Convert.ToInt32(tr) > 0 && ch.Equals("1") && dv.Equals("0"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm mười ";
                if (Convert.ToInt32(tr) > 0 && ch.Equals("1") && dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm mười lăm ";
            }
            return Ktach;
        }
        private static string Donvi(string so)
        {
            string Kdonvi = "";
            if (so.Equals("1"))
                Kdonvi = "";
            if (so.Equals("2"))
                Kdonvi = "nghìn";
            if (so.Equals("3"))
                Kdonvi = "triệu";
            if (so.Equals("4"))
                Kdonvi = "tỷ";
            if (so.Equals("5"))
                Kdonvi = "nghìn tỷ";
            if (so.Equals("6"))
                Kdonvi = "triệu tỷ";
            if (so.Equals("7"))
                Kdonvi = "tỷ tỷ";
            return Kdonvi;

        }
        public static string DocSoThanhChu(double gNum)
        {
            if (gNum == 0)
                return "Không đồng";
            string lso_chu = "";
            string tach_mod = "";
            string tach_conlai = "";
            double Num = Math.Round(gNum, 0);
            string gN = Convert.ToString(Num);
            int m = Convert.ToInt32(gN.Length / 3);
            int mod = gN.Length - m * 3;
            string dau = "[+]";
            // Dau [+ , - ]
            if (gNum < 0)
                dau = "[-]";
            dau = "";
            // Tach hang lon nhat
            if (mod.Equals(1))
                tach_mod = "00" + Convert.ToString(Num.ToString().Trim().Substring(0, 1)).Trim();
            if (mod.Equals(2))
                tach_mod = "0" + Convert.ToString(Num.ToString().Trim().Substring(0, 2)).Trim();
            if (mod.Equals(0))
                tach_mod = "000";
            // Tach hang con lai sau mod :
            if (Num.ToString().Length > 2)
                tach_conlai = Convert.ToString(Num.ToString().Trim().Substring(mod, Num.ToString().Length - mod)).Trim();
            ///don vi hang mod 
            int im = m + 1;
            if (mod > 0)
                lso_chu = Tach(tach_mod).ToString().Trim() + " " + Donvi(im.ToString().Trim());
            /// Tach 3 trong tach_conlai
            int i = m;
            int _m = m;
            int j = 1;
            string tach3 = "";
            string tach3_ = "";
            while (i > 0)
            {
                tach3 = tach_conlai.Trim().Substring(0, 3).Trim();
                tach3_ = tach3;
                lso_chu = lso_chu.Trim() + " " + Tach(tach3.Trim()).Trim();
                m = _m + 1 - j;
                if (!tach3_.Equals("000"))
                    lso_chu = lso_chu.Trim() + " " + Donvi(m.ToString().Trim()).Trim();
                tach_conlai = tach_conlai.Trim().Substring(3, tach_conlai.Trim().Length - 3);
                i = i - 1;
                j = j + 1;
            }
            if (lso_chu.Trim().Substring(0, 1).Equals("k"))
                lso_chu = lso_chu.Trim().Substring(10, lso_chu.Trim().Length - 10).Trim();
            if (lso_chu.Trim().Substring(0, 1).Equals("l"))
                lso_chu = lso_chu.Trim().Substring(2, lso_chu.Trim().Length - 2).Trim();
            if (lso_chu.Trim().Length > 0)
                lso_chu = dau.Trim() + " " + lso_chu.Trim().Substring(0, 1).Trim().ToUpper() + lso_chu.Trim().Substring(1, lso_chu.Trim().Length - 1).Trim() + " đồng.";
            return lso_chu.ToString().Trim();
        }
        public static string DocThanhChu(this int gNum)
        {
            return DocSoThanhChu(gNum);
        }
        public static string DocThanhChu(this float gNum)
        {
            return DocSoThanhChu(gNum);
        }
        #endregion
        #region "Ngày tháng"
        public static String Ngay_ky = "Ngày " + (DateTime.Now.Day < 10 ? ("0" + DateTime.Now.Day.ToString()) : DateTime.Now.Day.ToString())
   + " tháng " + (DateTime.Now.Month < 10 ? ("0" + DateTime.Now.Month.ToString()) : DateTime.Now.Month.ToString())
   + " năm " + DateTime.Now.Year.ToString();
        public static bool CheckDauThangCuoiThang(DateTime Tu_ngay, DateTime Den_ngay)
        {
            bool result = false;
            if (Tu_ngay.Month == Den_ngay.Month)
            {
                if (Tu_ngay.Day == 1 && Den_ngay.Day == DateTime.DaysInMonth(Tu_ngay.Year, Tu_ngay.Month))
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion
        #region "MD5"
        public static string CreateMD5(string input, string pk = "")
        {
            input = pk != "" ? (input + pk) : input;
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
        public static string ToMD5(this string input, bool withPK = true)
        {
            var pk = withPK ? "namvietjsc_essadmin" : "";
            return myExtension.CreateMD5(input, pk);
        }
        #endregion
        #region "Send Email"
        /// <summary>
        /// Kiểm tra email có tồn tại không
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this string email)
        {
            try
            {
                var foo = new EmailAddressAttribute();
                return foo.IsValid(email);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Gửi email tới danh sách người nhận không attach files
        /// </summary>
        /// <param name="Subject">Chủ đề</param>
        /// <param name="Body">Nội dung</param>
        /// <param name="listAddreses">Danh sách người nhận</param>
        /// <remarks></remarks>
        public static bool SendEmail(string Subject, string Body, System.Collections.Generic.List<string> listAddreses)
        {
            try
            {
                List<string> listFileNames = new List<string>();
                return SendEmail(Subject, Body, listAddreses, listFileNames);

            }
            catch (Exception )
            {
                return false;
            }
        }
        /// <summary>
        /// Gửi email tới danh sách người nhận có file đính kèm
        /// </summary>
        /// <param name="Subject">Chủ đề</param>
        /// <param name="Body">Nội dung</param>
        /// <param name="listAddreses">Danh sách người nhận</param>
        /// <param name="listFileNames">Danh sách file đính kèm</param>
        /// <remarks></remarks>
        public static bool SendEmail(string Subject, string Body, List<string> listAddreses, List<string> listFileNames)
        {
            try
            {
                String EmailAddress = System.Configuration.ConfigurationManager.ConnectionStrings["EmailAddress"].ToString();
                String PassWordEmail = System.Configuration.ConfigurationManager.ConnectionStrings["PassWordEmail"].ToString();
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(EmailAddress);
                mail.Subject = Subject;
                mail.Body = Body;
                //'
                foreach (string fileName in listFileNames)
                {
                    System.Net.Mail.Attachment Attachment = default(System.Net.Mail.Attachment);
                    Attachment = new System.Net.Mail.Attachment(fileName);
                    mail.Attachments.Add(Attachment);
                }
                //'
                if (listAddreses.Count == 0)
                {
                    return false;
                }
                foreach (string address in listAddreses)
                {
                    mail.To.Add(address);
                }
                //'
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(EmailAddress, PassWordEmail);
                SmtpServer.EnableSsl = true;
                //'
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }
        #endregion
        public static List<Type> GetAllController()
        {
            List<Type> listForm = new List<Type>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            IEnumerable<Type> types = assembly.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)) //&& type.Namespace.ToLower().Equals(nameSpace.ToLower())
                .OrderBy(x => x.Name);
            return types.ToList();
        }
        #region "Mã hóa giống  phần mềm"

        private static string HashData(HashAlgorithm algo, string data)
        {
            byte[] rawData = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
            byte[] result = algo.ComputeHash(rawData);
            return System.Convert.ToBase64String(result, 0, result.Length);
        }
        private static string MD5(string txt)
        {
            MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
            return HashData(_md5, txt);
        }
        public static string ToMD5_PhienBanPhanMem(this string input)
        {
            return myExtension.MD5(input);
        }
        #endregion

        //public static List<Action_Controller_Model> GetAllAction(Type controller)
        //{
        //    List<Action_Controller_Model> list = new List<Action_Controller_Model>();
        //    IEnumerable<MemberInfo> meberInfo = controller.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly
        //        | BindingFlags.Public).Where(x => !x.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
        //            true).Any());
        //    foreach (var item in meberInfo)
        //    {
        //        if (item.ReflectedType.IsPublic && !item.IsDefined(typeof(NonActionAttribute)))
        //        {
        //            Action_Controller_Model model = new Action_Controller_Model();
        //            model.ControllerName = controller.Name;
        //            model.Action_name = item.Name;
        //            list.Add(model);
        //        }
        //    }
        //    return list;
        //}
        //public static List<Action_Controller_Model> GetAllAction()
        //{
        //    List<Action_Controller_Model> list = new List<Action_Controller_Model>();
        //    foreach (Type item in GetAllController())
        //    {
        //        List<Action_Controller_Model> listAction = GetAllAction(item);
        //        foreach (var action in listAction)
        //        {
        //            list.Add(action);
        //        }
        //    }
        //    return list;
        //}

        public static string GetIPAddress()
        {
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return Convert.ToString(IP);
                }
            }
            return "";
        }
        public static string FromBase64String(this string input)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(input);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                return null;
            }
        }
    }
}
