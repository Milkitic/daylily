//using System.Collections.Generic;
//using System.Diagnostics;
//using Daylily.Bot.Backend;
//using Daylily.Common.Utils.LoggerUtils;
//using Daylily.CoolQ.Message;
//using Newtonsoft.Json;

//namespace Daylily.CoolQ.Plugin
//{
//    public class ExtendPlugin : CoolQCommandPlugin
//    {
//        public string Program { get; set; }
//        public string File { get; set; }

//        public override BackendConfig BackendConfig { get; } = new BackendConfig
//        {
//            Priority = 0
//        };

//        public sealed override Message.CommonMessageResponse OnMessageReceived(CoolQNavigableMessage navigableMessageObj)
//        {
//            return CreateProc(navigableMessageObj);
//        }

//        private Message.CommonMessageResponse CreateProc(CoolQNavigableMessage navigableMessageObj)
//        {
//            List<string> receivedString = new List<string>();
//            navigableMessageObj.Encode();
//            string arg = JsonConvert.SerializeObject(navigableMessageObj).Replace("\"", "\\\"").Replace(" ", "+++").Insert(0, "\"");
//            arg = arg + "\"";
//            Logger.Debug(arg);
//            var proc = new Process
//            {
//                StartInfo =
//                {
//                    FileName = Program,
//                    Arguments =$"{File} {arg}",
//                    CreateNoWindow = true,
//                    UseShellExecute = false,
//                    WindowStyle = ProcessWindowStyle.Hidden,
//                    RedirectStandardOutput = true,
//                    RedirectStandardError = true
//                }
//            };

//            proc.OutputDataReceived += (sender, e) =>
//            {
//                Logger.Debug(e.Data);
//                if (e.Data != null && e.Data.Trim().Trim('\n').Trim('\r') != "") receivedString.Add(e.Data);
//            };

//            proc.ErrorDataReceived += (sender, e) =>
//            {
//                Logger.Debug(e.Data);
//                if (e.Data != null && e.Data.Trim().Trim('\n').Trim('\r') != "") receivedString.Add(e.Data);
//            };

//            proc.Start();
//            proc.BeginOutputReadLine();
//            proc.BeginErrorReadLine();

//            proc.WaitForExit();
//            return ProcExited();

//            Message.CommonMessageResponse ProcExited()
//            {
//                if (receivedString.Count == 0)
//                    return null;
//                string last = receivedString[receivedString.Count - 1];
//                try
//                {
//                    return JsonConvert.DeserializeObject<Message.CommonMessageResponse>(last);
//                }
//                catch (JsonReaderException)
//                {
//                    Logger.Error("转换JSON失败，因此无法做出应答");
//                    return null;
//                }
//            }
//        }
//    }
//}
