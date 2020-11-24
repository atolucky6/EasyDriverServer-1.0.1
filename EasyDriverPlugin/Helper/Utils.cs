using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace EasyDriverPlugin
{
    public static class Utils
    {
        public const string LastOfStringIsNumberPattern = @"\d+$";
        public const string LastOfStringIsNumberOrNumberSurroundByBracketsPattern = @"((\d+)|([(]\d+[)]))$";
        public const string IpAddressPattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";

        /// <summary>
        /// Hàm lấy tên của class và tách ra nếu cần thiết.
        /// VD: ClassDoSomeThing = Class Do Some Thing
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        public static string GetClassName(this object obj, bool spacing = false)
        {
            var className = obj.GetType().Name;
            if (spacing)
            {
                var splitName = Regex.Split(className, @"(?<!^)(?=[A-Z])");
                var sb = new StringBuilder();
                foreach (var item in splitName)
                    sb.Append(item + " ");
                return sb.ToString().Trim();
            }
            return className;
        }

        /// <summary>
        /// Hàm so sánh 2 chuỗi byte
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static unsafe bool UnsafeComapre(this byte[] a1, byte[] a2)
        {
            if (a1 == a2)
                return true;
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*)x1) != *((long*)x2)) return false;
                if ((l & 4) != 0) { if (*((int*)x1) != *((int*)x2)) return false; x1 += 4; x2 += 4; }
                if ((l & 2) != 0) { if (*((short*)x1) != *((short*)x2)) return false; x1 += 2; x2 += 2; }
                if ((l & 1) != 0) if (*((byte*)x1) != *((byte*)x2)) return false;
                return true;
            }
        }

        /// <summary>
        /// Trích xuất chuỗi số cuối cùng của chuỗi hoặc số nằm trong dấu ()
        /// </summary>
        /// <param name="str"></param>
        /// <param name="hasValue"></param>
        /// <param name="hasBracketsSurround"></param>
        /// <returns></returns>
        public static uint ExtractLastNumberFromString(this string str, out bool hasValue, out bool hasBracketsSurround)
        {
            var extractStr = Regex.Match(str, @"((\d+)|([(]\d+[)]))$").ToString();
            hasValue = false;
            hasBracketsSurround = false;
            if (!string.IsNullOrEmpty(extractStr))
            {
                hasValue = true;
                if (hasBracketsSurround = extractStr.Contains('('))
                    extractStr = extractStr.Trim(new[] { '(', ')' });
                return uint.Parse(extractStr);
            }
            return 0;
        }

        /// <summary>
        /// Xóa chuỗi số hoặc chuỗi số nằm trong () khỏi chuỗi.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveLastNumberFromString(this string str)
        {
            return Regex.Replace(str, @"((\d+)|([(]\d+[)]))$", "");
        }

        /// <summary>
        /// Kiểm tra chuỗi có chứa ký tự nào trong danh sách hay không
        /// </summary>
        /// <param name="str"></param>
        /// <param name="filterChars"></param>
        /// <returns></returns>
        public static bool Contains(this string str, char[] filterChars)
        {
            foreach (var filterChar in filterChars)
            {
                if (str.Contains(filterChar))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Hàm chuyển danh sách ký tự thành một chuỗi
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        public static string ToString(this char[] chars, bool spacing = true)
        {
            if (spacing)
            {
                string result = string.Empty;
                foreach (var item in chars)
                    result += string.Format(" {0}", item);
                return result;
            }
            else
            {
                string result = string.Empty;
                foreach (var item in chars)
                    result += item;
                return result;
            }
        }

        /// <summary>
        /// Kiểm tra chuỗi có phải là định dạng địa chỉ Ip
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIpAddress(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return Regex.IsMatch(str, IpAddressPattern);
        }

        /// <summary>
        /// Kiểm tra giá trị có nằm trong khoảng 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsInRange(this int value, int min, int max)
        {
            if (value > max || value < min)
                return false;
            return true;
        }

        /// <summary>
        /// Kiểm tra giá trị có nằm trong khoảng 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsInRange(this double value, double min, double max)
        {
            if (value > max || value < min)
                return false;
            return true;
        }

        /// <summary>
        /// Kiểm tra giá trị có nằm trong khoảng 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsInRange(this float value, float min, float max)
        {
            if (value > max || value < min)
                return false;
            return true;
        }

        /// <summary>
        /// Kiểm tra số có phải là số chẵn
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEvenNumber(this int value)
        {
            if (value % 2 == 0)
                return true;
            return false;
        }

        /// <summary>
        /// Kiểm tra số có phải là số lẻ
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOddNumber(this int value)
        {
            return !IsEvenNumber(value);
        }
    }
}
