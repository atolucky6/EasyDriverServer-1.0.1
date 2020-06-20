using System.Text.RegularExpressions;

namespace EasyScada.ServerApplication
{
    public static class ValidateHelper
    {
        public static string REGEX_ValidFileName = @"^[\w\-. ]+$";

        public static string ValidateFileName(this string str)
        {
            str = str?.Trim();
            if (string.IsNullOrEmpty(str))
                return "The name can't be empty.";
            if (!Regex.IsMatch(str, REGEX_ValidFileName))
                return "The name was not in correct format.";
            return string.Empty;
        }
    }
}
