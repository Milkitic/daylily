using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using Daylily.Common.Assist;
using Daylily.Common.Models.MessageList;
using Daylily.Common.Utils.LogUtils;
using Newtonsoft.Json;

namespace Daylily.Common.Models.Interface
{
    public class ExtendApp : CommandApp
    {
        public string Program { get; set; }
        public string File { get; set; }

        public sealed override void Initialize(string[] args) { }

        public sealed override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            return CreateProc(messageObj);
        }

        private CommonMessageResponse CreateProc(in CommonMessage messageObj)
        {
            List<string> receivedString = new List<string>();
            messageObj.Encode();
            string arg = JsonConvert.SerializeObject(messageObj).Replace("\"", "\\\"").Replace(" ", "+++").Insert(0, "\"");
            arg = arg + "\"";
            Logger.Debug(arg);
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = Program,
                    Arguments =$"{File} {arg}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            proc.OutputDataReceived += (sender, e) =>
            {
                Logger.Debug(e.Data);
                if (e.Data != null && e.Data.Trim().Trim('\n').Trim('\r') != "") receivedString.Add(e.Data);
            };

            proc.ErrorDataReceived += (sender, e) =>
            {
                Logger.Debug(e.Data);
                if (e.Data != null && e.Data.Trim().Trim('\n').Trim('\r') != "") receivedString.Add(e.Data);
            };

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            proc.WaitForExit();
            return ProcExited();

            CommonMessageResponse ProcExited()
            {
                if (receivedString.Count == 0)
                    return null;
                string last = receivedString[receivedString.Count - 1];
                try
                {
                    return JsonConvert.DeserializeObject<CommonMessageResponse>(last);
                }
                catch (JsonReaderException)
                {
                    Logger.Error("转换JSON失败，因此无法做出应答");
                    return null;
                }
            }
        }
    }
}
