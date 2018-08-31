using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Daylily.Common.IO;

namespace Daylily.Common.Utils.LoggerUtils
{
    public static class Logger
    {
        internal static readonly LogLists LogLists = new LogLists();
        public static void Raw(string str)
        {
            Console.ResetColor();
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
            Console.ResetColor();
            string source = WriteSource();
            string info = WriteInfo(msg);
            LogLists.OriginList.Add(source, info);
        }

        public static void Message(string msg)
        {
            Console.ResetColor();
            string source = WriteSource(true);
            string info = WriteInfo(msg);
            LogLists.MessageList.Add(source, info);
        }

        public static void Debug(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            string source = WriteSource();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.DebugList.Add(source, info);
        }

        public static void Info(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            string source = WriteSource();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.InfoList.Add(source, info);
        }

        public static void Warn(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            string source = WriteSource();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.WarnList.Add(source, info);
        }

        public static void Error(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Gray;
            string source = WriteSource();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.ErrorList.Add(source, info);
        }

        public static void Success(string msg)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Gray;
            string source = WriteSource();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.SuccessList.Add(source, info);
        }

        public static void Exception(Exception ex)
        {
            WriteToLog(ex.ToString());
            Error(Environment.NewLine + ex, 1);
        }

        public static void Exception(Exception ex, string pluginInfo, string pluginName)
        {
            if (pluginInfo.Length >= 1000)
                pluginInfo = pluginInfo.Remove(1000) + ".....(Too long)";
            string pluginHint = Environment.NewLine +
                                $"\"/{pluginInfo}\" caused an exception: {Environment.NewLine} ---> {pluginName}:" +
                                Environment.NewLine;

            string contents = pluginHint + ex;
            WriteToLog(contents);
            Error(contents, 1);
        }

        private static string WriteSource(bool ignoreMethod = false, int offset = 0)
        {
            string methodName = "";
            string time = "";
            if (!ignoreMethod)
            {
                StackTrace st = new StackTrace(true);
                MethodBase mb = st.GetFrame(2 + offset).GetMethod();
                methodName = $"[{mb.DeclaringType.Namespace}.{mb.DeclaringType.Name}.{mb.Name}]";
            }
            else
            {
                var n = DateTime.Now;
                time = $"[{n.Hour:00}:{n.Minute:00}:{n.Second:00}]";
            }

            string writeStr = time + methodName;
            Console.Write(writeStr);
            Console.ResetColor();
            //Console.WriteLine();
            Console.Write(" ");
            return writeStr;
        }

        private static string WriteInfo(string msg)
        {
            if (msg != null && msg.Length >= 2000) msg = msg.Remove(2000) + ".....(Too long)";
            string writeStr = msg;
            Console.WriteLine(writeStr);
            Console.ResetColor();
            return writeStr;
        }

        private static void WriteToLog(string contents)
        {
            contents = string.Format("-----{0} {1}{2}{3}{4}", DateTime.Now.ToLongDateString(),
                DateTime.Now.ToLongTimeString(), Environment.NewLine, contents, Environment.NewLine + Environment.NewLine);
            var logPath = Domain.LogPath;
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            ConcurrentFile.AppendAllText(Path.Combine(logPath, "exception.log"), contents);
        }

        private static void Error(string msg, int offset)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Gray;
            string source = WriteSource(offset: offset);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            string info = WriteInfo(msg);
            LogLists.ErrorList.Add(source, info);
        }
    }
}
