using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.RequestUtils;
using Daylily.CoolQ.CoolQHttp;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Daylily.Plugin.ShaDiao.Application
{
    [Name("嗅探龙图")]
    [Author("yf_extension", "sahuang")]
    [Version(2, 0, 1, PluginVersion.Alpha)]
    [Help("发现龙图时作出回应。")]
    class DragonDetectorAlpha : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("44ec1c54-8a34-4e8d-82b8-cf3c5babf88e");

        private readonly List<string> _pathList = new List<string>();
        private readonly List<string> _receivedString = new List<string>();

        private Thread _thread;
        private Process _proc;

        private static int _totalCount;
        private int _currentCount;

        private string _user, _group;
        private long _messageId;

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            if (routeMsg.Group == null) return null;

            //if (user != "2241521134") return null;
            _user = routeMsg.UserId;
            _group = routeMsg.GroupId;
            _messageId = routeMsg.MessageId;

            var imgList = CoolQCode.GetImageInfo(routeMsg.Message.RawMessage);
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
                    var root = WebRequestUtil.GetImageFromUrl(item.Url, item.Md5, item.Extension);
                    _pathList.Add(root);
                }

                _totalCount++;
            }

            _thread = new Thread(RunDetector);
            _thread.Start(_pathList);
            Logger.Warn("已经发送了请求,目前队列中共" + _totalCount);
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
                {
                    //Thread.Sleep(6000);
                    //continue;
                    if (_proc != null)
                    {
                        if (!_proc.HasExited) _proc.Kill();
                        _proc = null;
                    }

                    _proc = new Process
                    {
                        StartInfo =
                        {
                            FileName = "python3", // python3 dragon-detection.py "root"
                            Arguments =
                                $"{Path.Combine(Domain.PluginPath, "dragon", "dragon-detection.py")} \"{fullPath}\"",
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardOutput = true, // 重定向标准输出  
                            RedirectStandardError = true // 重定向错误输出  
                            //StartInfo.StandardOutputEncoding = Encoding.UTF8;
                        }
                    };

                    _proc.OutputDataReceived += ProcOutputReceived;
                    _proc.ErrorDataReceived += ProcErrorReceived;

                    Logger.Origin("正在调用中");
                    _proc.Start();
                    _proc.BeginOutputReadLine();
                    _proc.BeginErrorReadLine();

                    _proc.WaitForExit();
                    ProcExited();
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
                finally
                {
                    _totalCount--;
                }
            }

            if (_currentCount <= 0) return;

            //CqApi.SetGroupBan(group, user, rnd.Next(1, 100 * dragonCount + 1) * 60);
            if (_group != "133605766")
                CoolQHttpApi.SendGroupMessageAsync(_group, new At(_user) + " 你龙了?");
            else
                CoolQHttpApi.DeleteMessage(_messageId);
            if (_currentCount <= 1) return;
            Thread.Sleep(8000);
            CoolQHttpApi.SetGroupBan(_group, _user, StaticRandom.Next(1, 100 * _currentCount + 1) * 60);
            //CqApi.SendGroupMessageAsync(group, "而且有好多张，送你" + dragonCount + "倍套餐!!");
        }

        private void ProcOutputReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Trim() == "") return;
            _receivedString.Add(e.Data);
        }

        private void ProcErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Trim() == "") return;
            _receivedString.Add(e.Data);
        }

        private void ProcExited()
        {
            if (_receivedString.Count == 0) return;
            string line = _receivedString[_receivedString.Count - 1];
            Logger.Warn(line);

            var tmp = line.Split(' ');
            var status = int.Parse(tmp[0]);
            var confidence = double.Parse(tmp[1]);
            if (status == 1 && confidence > 68)
                _currentCount++;
        }
    }
}
