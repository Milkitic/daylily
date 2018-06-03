using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Daylily.Common.Assist
{
    public static class Logger
    {
        /// <summary>
        /// 记录日志至控制台
        /// </summary>
        /// <param name="msg">消息</param>
        public static void WriteLine(string msg)
        {
            Console.ResetColor();
            WriteSource();
            WriteInfo(msg);
        }

        public static void WriteMessage(string msg)
        {
            Console.ResetColor();
            WriteSource(true);
            WriteInfo(msg);
        }

        public static void WriteException(Exception ex)
        {
            DangerLine(Environment.NewLine + ex, 1);
        }

        public static void WriteException(Exception ex, string pluginInfo, string pluginName)
        {
            if (pluginInfo.Length >= 1000)
                pluginInfo = pluginInfo.Remove(1000) + ".....(Too long)";
            string pluginHint = Environment.NewLine +
                                $"\"/{pluginInfo}\" caused an exception: {Environment.NewLine} ---> {pluginName}:" +
                                Environment.NewLine;

            //if (ex.InnerException != null)
            //    DangerLine(pluginHint + ex.InnerException, 1);
            //else
            DangerLine(pluginHint + ex, 1);
        }

        public static void DefaultLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            WriteSource();

            WriteInfo(msg);
        }

        public static void DebugLine(string msg)
        {
#if DEBUG
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            WriteSource();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteInfo(pluginInfo);
#endif
        }

        public static void PrimaryLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteSource();

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteInfo(msg);
        }

        public static void SuccessLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteSource();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteInfo(msg);
        }

        public static void InfoLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            WriteSource();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteInfo(msg);
        }

        public static void WarningLine(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            WriteSource();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteInfo(msg);
            Console.ResetColor();
        }

        public static void DangerLine(string msg, int offset = 0)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteSource(offset: offset);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            WriteInfo(msg);
        }

        private static void WriteSource(bool ignoreMethod = false, int offset = 0)
        {
            string methodName = "";
            if (!ignoreMethod)
            {
                StackTrace st = new StackTrace(true);
                MethodBase mb = st.GetFrame(2 + offset).GetMethod();
                methodName = $"[{mb.DeclaringType.Namespace}.{mb.DeclaringType.Name}.{mb.Name}]";
            }

            var n = DateTime.Now;
            Console.Write($"[{n.Hour:00}:{n.Minute:00}:{n.Second:00}]{methodName}");
            Console.ResetColor();
        }

        private static void WriteInfo(string msg)
        {
            if (msg.Length >= 2000)
                msg = msg.Remove(2000) + ".....(Too long)";
            Console.WriteLine(" " + msg);
            Console.ResetColor();
        }
    }
}
