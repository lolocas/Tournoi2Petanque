using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class StringExtension
    {    
        /// <summary>
        /// Renvoit la chaine avec la première lettre en majuscule
        /// </summary>
        /// <param name="p_strValue"></param>
        /// <returns></returns>
        public static string FirstLetterUpper(this string p_strValue)
        {
            if (string.IsNullOrEmpty(p_strValue))
                return string.Empty;
            char[] l_chrValue = p_strValue.ToLower().ToCharArray();
            l_chrValue[0] = char.ToUpper(l_chrValue[0]);

            return new string(l_chrValue);
        }
    }
}
