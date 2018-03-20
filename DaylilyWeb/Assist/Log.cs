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
        public static void WriteLine(string msg, string source = "default")
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}][{source}] {msg}");
        }
    }
}
