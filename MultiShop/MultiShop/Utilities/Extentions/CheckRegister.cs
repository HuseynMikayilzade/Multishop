using System.Text.RegularExpressions;

namespace MultiShop.Utilities.Extentions
{
    public static class CheckRegister
    {
        public static bool isDigit(this string name)
        {
            return (name.Any(char.IsDigit));           
        }
        public static string Capitalize(this string name)
        {
            name = name.Trim();
            name = name.Substring(0,1).ToUpper() + name.Substring(1).ToLower();
            return name;
        }

        public static bool CheckEmail(this string email)
        {
            //return email.Contains(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (string.IsNullOrEmpty(email)) return false;

            string emailregex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex regex = new Regex(emailregex);
            return regex.IsMatch(email);
          
        }
    }
}
