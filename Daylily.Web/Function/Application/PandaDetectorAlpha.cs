using Daylily.Common.Assist;
using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Web.Function.Application
{
    public class PandaDetectorAlpha : AppConstruct
    {
        public override string Name => "斗图";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Beta;
        public override string VersionNumber => "1.0";
        public override string Description => "发现各类熊猫图时有几率返回一张熊猫图";
        public override string Command => null;
        public override AppType AppType => AppType.Application;

        private readonly List<string> _receivedString = new List<string>();
        private readonly List<string> _pathList = new List<string>();

        private Thread _thread;
        private Process _proc;

        private int _pandaCount;
        private static int _totalCount;

        private CommonMessage _message;

        public override void OnLoad(string[] args)
        {
            throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage message)
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

            _thread = new Thread(RunDetector);
            _thread.Start(_pathList);
            Logger.PrimaryLine("(熊猫) 共 " + _totalCount);
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

                    _proc = new Process
                    {
                        StartInfo =
                        {
                            FileName = "python3",
                            Arguments =
                                $"{Path.Combine(Environment.CurrentDirectory, "dragon", "panda-detection.py")} \"{fullPath}\"",
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                    };
                    _proc.OutputDataReceived += ProcOutputReceived;
                    _proc.ErrorDataReceived += ProcErrorReceived;

                    _proc.Start();
                    _proc.BeginOutputReadLine();
                    _proc.BeginErrorReadLine();

                    _proc.WaitForExit();
                    ProcExited();
                }
                catch (Exception ex)
                {
                    Logger.WriteException(ex);
                }
                finally
                {
                    _totalCount--;
                    Logger.PrimaryLine("(熊猫) " + (_totalCount + 1) + " ---> " + _totalCount);
                }
            }

            if (_pandaCount <= 0) return;

            var perc = Rnd.NextDouble();
            if (perc < 0.15 || (perc < 0.5 && _message.GroupId == "428274344"))
            {
                Logger.SuccessLine("(熊猫) 几率: " + perc);
                DirectoryInfo di =
                    new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "dragon", "resource_panda_send"));
                var files = di.GetFiles();
                string msg = CqCode.EncodeFileToBase64(files[Rnd.Next(files.Length)].FullName);
                SendMessage(new CommonMessageResponse(msg, _message));
            }
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
            //Logger.WarningLine(line);

            var tmp = line.Split(' ');
            var status = int.Parse(tmp[0]);
            var confidence = double.Parse(tmp[1]);
            if (status == 1 && confidence > 50)
                _pandaCount++;
        }
    }
}
