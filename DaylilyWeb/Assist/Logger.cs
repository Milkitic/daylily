using System;
using System.Diagnostics;
using System.Reflection;

namespace DaylilyWeb.Assist
{
    public static class Logger
    {

        /// <summary>
        /// 记录日志至控制台
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="source">程序模块名称</param>
        public static void WriteLine(string msg, string source = "default", string function = "")
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}][{source + (function != "" ? "." + function : "")}] {msg}");
        }
        public static void DefaultLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            WriteSource();
            Console.ResetColor();

            WriteMessage(msg);
        }
        public static void PrimaryLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteSource();
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteMessage(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void SuccessLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteSource();
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteMessage(msg);
            Console.ResetColor();
        }
        public static void InfoLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            WriteSource();
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteMessage(msg);
            Console.ResetColor();
        }
        public static void WarningLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteSource();
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteMessage(msg);
            Console.ResetColor();
        }
        public static void DangerLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteSource();
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteMessage(msg);
            Console.ResetColor();
        }
        private static void WriteSource()
        {
            StackTrace st = new StackTrace(true);
            MethodBase mb = st.GetFrame(3).GetMethod();
            Console.Write($"[{DateTime.Now.ToString()}][{mb.DeclaringType.Namespace}.{mb.DeclaringType.Name}.{mb.Name}]");
        }
        private static void WriteMessage(string msg)
        {
            Console.WriteLine(" " + msg);
        }
    }
}
