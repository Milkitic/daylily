using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    public static class Mapper
    {
        static Dictionary<string, string> dClassName = new Dictionary<string, string>();

        public static void Init()
        {
            dClassName.Add("roll", "Roll");
            dClassName.Add("ping", "Ping");
        }
        public static string GetClassName(string name)
        {
            if (!dClassName.Keys.Contains(name))
                return null;
            return dClassName[name];
        }
    }
}
