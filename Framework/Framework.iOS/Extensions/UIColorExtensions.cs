using MonoTouch.UIKit;
using System;

namespace Framework.Extensions
{
    public static class UIColorExtensions
    {
        public static UIColor FromHexString(this string hexValue)
        {
            float red, green, blue;
            string colorString = hexValue.Replace("#", "");
            int l_intDecalage = 0;

            if (colorString.Length == 8)
                l_intDecalage = 2;

            red = Convert.ToInt32(colorString.Substring(l_intDecalage, 2), 16) / 255f;
            green = Convert.ToInt32(colorString.Substring(l_intDecalage + 2, 2), 16) / 255f;
            blue = Convert.ToInt32(colorString.Substring(l_intDecalage + 4, 2), 16) / 255f;
            return UIColor.FromRGBA(red, green, blue, 1.0f);
        }
    }
}