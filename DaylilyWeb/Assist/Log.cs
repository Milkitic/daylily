using System;

namespace DaylilyWeb.Assist
{
    public static class Log
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
        public static void DefaultLine(string msg, string source = "default", string function = "")
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            WriteLine(msg, source, function);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void PrimaryLine(string msg, string source = "default", string function = "")
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteLine(msg, source, function);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void SuccessLine(string msg, string source = "default", string function = "")
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteLine(msg, source, function);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void InfoLine(string msg, string source = "default", string function = "")
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            WriteLine(msg, source, function);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void WarningLine(string msg, string source = "default", string function = "")
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteLine(msg, source, function);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void DangerLine(string msg, string source = "default", string function = "")
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteLine(msg, source, function);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }

    }
}
