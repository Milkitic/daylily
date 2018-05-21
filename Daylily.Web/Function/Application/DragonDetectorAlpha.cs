using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Daylily.Common.Models;
using Daylily.Common.Assist;

namespace Daylily.Web.Function.Application
{
    public class DragonDetectorAlpha : AppConstruct
    {
        private readonly List<string> _pathList = new List<string>();
        private readonly List<string> _receivedString = new List<string>();

        private Thread _thread;
        private Process _proc;

        private static int _totalCount;
        private int _currentCount;
        
        private string _user, _group;
        private long _messageId;

        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.Group == null && message.GroupId != "133605766") return null;

            //if (user != "2241521134") return null;
            _user = message.UserId;
            _group = message.GroupId;
            _messageId = message.MessageId;

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
            Logger.WarningLine("已经发送了请求,目前队列中共" + _totalCount);
            return null;
        }

        /// <summary>
        /// 核心识别by sahuang
        /// </summary>
        private void RunDetector(object pathList)
        {

            var list = (List<string>)pathList;
            foreach (var fullPath in list)
            {
                try
                {     //Thread.Sleep(6000);
                    //continue;
                    if (_proc != null)
                    {
                        if (!_proc.HasExited) _proc.Kill();
                        _proc = null;
                    }
                    _proc = new Process();
                    _proc.StartInfo.FileName = "python3";  // python3 dragon-detection.py "root"
                    _proc.StartInfo.Arguments = $"{Path.Combine(Environment.CurrentDirectory, "dragon", "dragon-detection.py")} \"{fullPath}\"";      // 参数  

                    _proc.StartInfo.CreateNoWindow = true;
                    //proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    _proc.StartInfo.UseShellExecute = false;
                    _proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    _proc.StartInfo.RedirectStandardOutput = true; // 重定向标准输出  
                    _proc.StartInfo.RedirectStandardError = true;  // 重定向错误输出  
                    _proc.OutputDataReceived += ProcOutputReceived;
                    _proc.ErrorDataReceived += ProcErrorReceived;

                    Console.WriteLine("正在调用中");
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
                }
            }

            if (_currentCount > 0)
            {
                CqApi.DeleteMessage(_messageId);

                //CQApi.SetGroupBan(group, user, rnd.Next(1, 100 * dragonCount + 1) * 60);
                //if (group != "133605766")
                //    CQApi.SendGroupMessageAsync(group, CQCode.EncodeAt(user) + " 你龙了?");
                if (_currentCount > 1)
                {
                    Thread.Sleep(8000);
                    CqApi.SetGroupBan(_group, _user, Rnd.Next(1, 100 * _currentCount + 1) * 60);
                    //CQApi.SendGroupMessageAsync(group, "而且有好多张，送你" + dragonCount + "倍套餐!!");
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
            if (status == 1 && confidence > 68)
            {
                //Logger.WarningLine(confidence.ToString());
                _currentCount++;
            }
            Console.WriteLine("调用结束");
        }
    }
}
