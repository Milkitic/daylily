using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly List<string> _receivedString = new List<string>();
        public sealed override void Initialize(string[] args) { }

        public sealed override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            return CreateProc(messageObj);
        }

        private CommonMessageResponse CreateProc(in CommonMessage messageObj)
        {
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = Program,
                    Arguments =$"{File} {JsonConvert.SerializeObject(messageObj)}",
                    //FileName = "ping",
                    //Arguments = "127.0.0.1",
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
                if (e.Data != null && e.Data.Trim() != "") _receivedString.Add(e.Data);
            };

            proc.ErrorDataReceived += (sender, e) =>
            {
                Logger.Debug(e.Data);
                if (e.Data != null && e.Data.Trim() != "") _receivedString.Add(e.Data);
            };

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            proc.WaitForExit();
            return ProcExited();
        }

        private CommonMessageResponse ProcExited()
        {
            if (_receivedString.Count == 0)
                return null;
            string last = _receivedString[_receivedString.Count - 1];
            return JsonConvert.DeserializeObject<CommonMessageResponse>(last);
        }
    }
}
