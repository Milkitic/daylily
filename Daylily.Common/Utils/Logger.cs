using System;
using System.Diagnostics;
using System.Reflection;

namespace Daylily.Common.Utils
{
    public static class Logger
    {
        internal static readonly LogLists LogLists = new LogLists();
        public static void Raw(string str)
        {
            System.Console.ResetColor();
            //string source = WriteSource();
            string info = WriteInfo(str);
            LogLists.RawList.Add(null, info);
        }
        /// <summary>
        /// 记录日志至控制台
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Origin(string msg)
        {
            System.Console.ResetColor();
            string source = WriteSource();
            string info = WriteInfo(msg);
            LogLists.OriginList.Add(source, info);
        }

        public static void Message(string msg)
        {
            System.Console.ResetColor();
            string source = WriteSource(true);
            string info = WriteInfo(msg);
            LogLists.MessageList.Add(source, info);
        }

        public static void Debug(string msg)
        {
            System.Console.BackgroundColor = ConsoleColor.Cyan;
            System.Console.ForegroundColor = ConsoleColor.Black;
            string source = WriteSource();

            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.DebugList.Add(source, info);
        }

        public static void Info(string msg)
        {
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.ForegroundColor = ConsoleColor.White;
            string source = WriteSource();

            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.InfoList.Add(source, info);
        }

        public static void Warn(string msg)
        {
            System.Console.BackgroundColor = ConsoleColor.Yellow;
            System.Console.ForegroundColor = ConsoleColor.Black;
            string source = WriteSource();

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.WarnList.Add(source, info);
        }

        public static void Error(string msg)
        {
            System.Console.BackgroundColor = ConsoleColor.Red;
            System.Console.ForegroundColor = ConsoleColor.Gray;
            string source = WriteSource();

            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.ErrorList.Add(source, info);
        }

        public static void Success(string msg)
        {
            System.Console.BackgroundColor = ConsoleColor.DarkGreen;
            System.Console.ForegroundColor = ConsoleColor.Gray;
            string source = WriteSource();

            System.Console.ForegroundColor = ConsoleColor.DarkGreen;
            System.Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.SuccessList.Add(source, info);
        }

        public static void Exception(Exception ex)
        {
            Error(Environment.NewLine + ex, 1);
        }

        public static void Exception(Exception ex, string pluginInfo, string pluginName)
        {
            if (pluginInfo.Length >= 1000)
                pluginInfo = pluginInfo.Remove(1000) + ".....(Too long)";
            string pluginHint = Environment.NewLine +
                                $"\"/{pluginInfo}\" caused an exception: {Environment.NewLine} ---> {pluginName}:" +
                                Environment.NewLine;

            //if (ex.InnerException != null)
            //    DangerLine(pluginHint + ex.InnerException, 1);
            //else
            Error(pluginHint + ex, 1);
        }

        private static string WriteSource(bool ignoreMethod = false, int offset = 0)
        {
            string methodName = "";
            if (!ignoreMethod)
            {
                StackTrace st = new StackTrace(true);
                MethodBase mb = st.GetFrame(2 + offset).GetMethod();
                methodName = $"[{mb.DeclaringType.Namespace}.{mb.DeclaringType.Name}.{mb.Name}]";
            }

            var n = DateTime.Now;
            string writeStr = $"[{n.Hour:00}:{n.Minute:00}:{n.Second:00}]{methodName}";
            System.Console.Write(writeStr);
            System.Console.ResetColor();
            System.Console.Write(" ");
            return writeStr;
        }

        private static string WriteInfo(string msg)
        {
            if (msg != null && msg.Length >= 2000) msg = msg.Remove(2000) + ".....(Too long)";
            string writeStr = msg;
            System.Console.WriteLine(writeStr);
            System.Console.ResetColor();
            return writeStr;
        }

        private static void Error(string msg, int offset)
        {
            System.Console.BackgroundColor = ConsoleColor.Red;
            System.Console.ForegroundColor = ConsoleColor.Gray;
            string source = WriteSource(offset: offset);

            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.ErrorList.Add(source, info);
        }
    }
}
