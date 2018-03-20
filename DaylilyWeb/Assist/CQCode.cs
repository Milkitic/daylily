using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    public class CQCode
    {
        public static string GetAt(string id)
        {
            return "[" + Escape($"CQ:at,qq={id}") + "]";
        }
        public static string Escape(string text)
        {
            return text.Replace("&", "&amp").Replace("[", "&#91").Replace("]", "&#93");
        }
    }
}
