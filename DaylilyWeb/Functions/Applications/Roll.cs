using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions.Applications
{
    public class Roll
    {
        static Random rnd = new Random();

        public static int Next()
        {
            return rnd.Next(0, 101);
        }

        public static int Next(int uBound)
        {
            return rnd.Next(0, uBound + 1);
        }

        public static int Next(int lBound, int uBound)
        {
            return rnd.Next(lBound, uBound + 1);
        }
    }
}
