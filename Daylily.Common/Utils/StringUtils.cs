using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Common.Utils
{
    public static class StringUtils
    {
        public static string GetUnderLineString(string source)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirstUpper = true;
            foreach (var c in source)
            {
                if (c >= 65 && c <= 90)
                {
                    if (!isFirstUpper)
                        sb.Append("_");
                    else
                        isFirstUpper = false;
                    sb.Append(c.ToString().ToLower());
                }
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
