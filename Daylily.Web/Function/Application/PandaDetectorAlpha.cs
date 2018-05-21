using Daylily.Common.Assist;
using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Daylily.Web.Function.Application
{
    public class PandaDetectorAlpha : AppConstruct
    {
        private readonly List<string> _receivedString = new List<string>();
        private readonly List<string> _pathList = new List<string>();

        private Thread _thread;
        private Process _proc;

        private int _pandaCount;
        private static int _totalCount;

        private CommonMessage _message;

        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.GroupId == "133605766")
                if (DateTime.Now.Hour < 22 && DateTime.Now.Hour > 6)
                    return null;

            //if (user != "2241521134") return null;
            _message = message;

            var imgList = CqCode.GetImageInfo(message.Message);
            if (imgList == null)
                return null;

            foreach (var item in imgList)
            {
                if (item.Extension.ToLower() == ".gif")
                    continue;
                if (item.FileInfo.Exists)
                {
                    _pathList.Add(item.FileInfo.FullName);
                }
                else
                {
                    WebRequestHelper.GetImageFromUrl(item.Url, item.Md5, item.Extension);
                    _pathList.Add(Path.Combine(Environment.CurrentDirectory, "images", item.Md5 + item.Extension));
                }
                _totalCount++;
            }
            _thread = new Thread(new ParameterizedThreadStart(RunDetector));
            _thread.Start(_pathList);
            Logger.PrimaryLine("熊猫共" + _totalCount);
            return null;
        }


        /// <summary>
        /// 核心识别by sahuang
        /// </summary>
        private void RunDetector(object newPathList)
        {

            var list = (List<string>)newPathList;
            foreach (var fullPath in list)
            {
                try
                {
                    if (_proc != null)
                    {
                        if (!_proc.HasExited) _proc.Kill();
                        _proc = null;
                    }
                    _proc = new Process();
                    _proc.StartInfo.FileName = "python3";  // python3 dragon-detection.py "root"
                    _proc.StartInfo.Arguments = $"{Path.Combine(Environment.CurrentDirectory, "dragon", "panda-detection.py")} \"{fullPath}\"";      // 参数  

                    _proc.StartInfo.CreateNoWindow = true;
                    //proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    _proc.StartInfo.UseShellExecute = false;
                    _proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    _proc.StartInfo.RedirectStandardOutput = true; // 重定向标准输出  
                    _proc.StartInfo.RedirectStandardError = true;  // 重定向错误输出  
                    _proc.OutputDataReceived += ProcOutputReceived;
                    _proc.ErrorDataReceived += ProcErrorReceived;

                    Logger.WriteLine("(熊猫)正在调用中");
                    _proc.Start();
                    _proc.BeginOutputReadLine();
                    _proc.BeginErrorReadLine();

                    _proc.WaitForExit();
                    ProcExited();
                }
                catch (Exception ex)
                {
                    Logger.DangerLine(ex.Message);
                }
                finally
                {
                    _totalCount--;
                    Logger.PrimaryLine("熊猫" + (_totalCount + 1) + "->" + _totalCount);
                }
            }

            if (_pandaCount > 0)
            {
                var perc = Rnd.NextDouble();
                if (perc < 0.15 || (perc < 0.5 && _message.GroupId == "428274344"))
                {
                    DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "dragon", "resource_panda_send"));
                    var files = di.GetFiles();
                    string msg = CqCode.EncodeFileToBase64(files[Rnd.Next(files.Length)].FullName);
                    SendMessage(new CommonMessageResponse(msg, _message));
                }
                else
                {
                    Logger.WarningLine("几率不够，没有触发：" + perc);
                }
            }

        }

        private void ProcOutputReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Trim() == "") return;
            _receivedString.Add(e.Data);
            //Console.WriteLine(e.Data);
        }
        private void ProcErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Trim() == "") return;
            _receivedString.Add(e.Data);
            //Console.WriteLine(e.Data);
        }
        private void ProcExited()
        {
            int status;
            double confidence;

            //receivedString.RemoveAll(x => x == null);

            if (_receivedString.Count == 0) return;
            string line = _receivedString[_receivedString.Count - 1];
            Logger.WarningLine(line);

            var tmp = line.Split(' ');
            status = int.Parse(tmp[0]);
            confidence = double.Parse(tmp[1]);
            if (status == 1 && confidence > 50)
            {
                //Logger.WarningLine(confidence.ToString());
                _pandaCount++;
            }
            Console.WriteLine("(熊猫)调用结束");
        }
    }
}
