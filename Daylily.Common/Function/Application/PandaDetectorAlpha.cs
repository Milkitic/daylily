using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Common.Function.Application
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

        private static readonly Dictionary<string, GroupSettings> GroupDic = new Dictionary<string, GroupSettings>();

        private static int _totalCount;

        public override void OnLoad(string[] args)
        {
            //throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(in CommonMessage messageObj)
        {
            if (messageObj.MessageType == MessageType.Private) return null;
            if (messageObj.GroupId == "133605766")
                if (DateTime.Now.Hour < 22 && DateTime.Now.Hour > 6)
                    return null;

            //if (user != "2241521134") return null;
            string groupId = messageObj.GroupId ?? messageObj.DiscussId;

            if (!GroupDic.ContainsKey(groupId))
                GroupDic.Add(groupId, new GroupSettings
                {
                    GroupId = groupId,
                    MessageObj = messageObj
                });
            //GroupDic[groupId].GroupType = messageObj.GroupId == null ? MessageType.Discuss : MessageType.Group;

            var imgList = CqCode.GetImageInfo(messageObj.Message);
            if (imgList == null)
                return null;

            foreach (var item in imgList)
            {
                if (item.Extension.ToLower() == ".gif")
                    continue;
                if (item.FileInfo.Exists)
                {
                    GroupDic[groupId].PathQueue.Enqueue(item.FileInfo.FullName);
                }
                else
                {
                    WebRequestHelper.GetImageFromUrl(item.Url, item.Md5, item.Extension);
                    GroupDic[groupId].PathQueue.Enqueue(Path.Combine(Domain.CurrentDirectory, "images",
                        item.Md5 + item.Extension));
                }

                _totalCount++;
            }

            if (GroupDic[groupId].Thread == null)
            {
                GroupDic[groupId].Thread = new Thread(RunDetector);
                GroupDic[groupId].Thread.Start(GroupDic[groupId]);
            }
            else
            {
                if (GroupDic[groupId].Thread.IsAlive) return null;

                GroupDic[groupId].Thread = new Thread(RunDetector);
                GroupDic[groupId].Thread.Start(GroupDic[groupId]);
                Logger.PrimaryLine("[" + groupId + "] (熊猫) 共 " + _totalCount);
            }

            return null;
        }

        /// <summary>
        /// 核心识别by sahuang
        /// </summary>
        private static void RunDetector(object groupSets)
        {
            var gSets = (GroupSettings)groupSets;

            while (gSets.PathQueue.Count != 0)
            {
                try
                {
                    CreateProc(gSets);
                    ProcExited(gSets);
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

            if (gSets.PandaCount < 1) return;
            Logger.PrimaryLine("[" + gSets.GroupId + "] (熊猫) " + gSets.PandaCount);

            for (int i = 0; i < gSets.PandaCount; i++)
            {
                var perc = Rnd.NextDouble();
                if (perc >= 0.15)
                    continue;
                Logger.SuccessLine("[" + gSets.GroupId + "] (熊猫) 几率: " + perc);

                string resPath = Path.Combine(Domain.CurrentDirectory, "dragon", "resource_panda_send");
                FileInfo[] files = new DirectoryInfo(resPath).GetFiles();
                string msg = CqCode.EncodeFileToBase64(files[Rnd.Next(files.Length)].FullName);
                SendMessage(new CommonMessageResponse(msg, gSets.MessageObj));
            }

            gSets.PandaCount = 0;
        }

        private static void CreateProc(GroupSettings gSets)
        {
            if (gSets.Process != null)
            {
                try
                {
                    if (!gSets.Process.HasExited) gSets.Process.Kill();
                }
                catch
                {
                    // ignored
                }

                gSets.Process = null;
            }

            string fullPath = gSets.PathQueue.Dequeue();

            gSets.Process = new Process
            {
                StartInfo =
                {
                    FileName = "python3",
                    Arguments =
                        $"{Path.Combine(Domain.CurrentDirectory, "dragon", "panda-detection.py")} \"{fullPath}\"",
                    //FileName = "ping",
                    //Arguments = "127.0.0.1",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            gSets.Process.OutputDataReceived += (sender, e) =>
            {
                Logger.DebugLine(e.Data);
                if (e.Data != null && e.Data.Trim() != "") gSets.ReceivedString.Add(e.Data);
            };

            gSets.Process.ErrorDataReceived += (sender, e) =>
            {
                Logger.DebugLine(e.Data);
                if (e.Data != null && e.Data.Trim() != "") gSets.ReceivedString.Add(e.Data);
            };

            gSets.Process.Start();
            gSets.Process.BeginOutputReadLine();
            gSets.Process.BeginErrorReadLine();

            gSets.Process.WaitForExit();
        }

        private static void ProcExited(GroupSettings gSets)
        {
            if (gSets.ReceivedString.Count == 0) return;
            string line = gSets.ReceivedString[gSets.ReceivedString.Count - 1];
            //Logger.WarningLine(line);

            var tmp = line.Split(' ');
            if (int.TryParse(tmp[0], out int status))
            {
                if (double.TryParse(tmp[1], out double confidence))
                {
                    if (status == 1 && confidence > 50)
                        gSets.PandaCount++;
                    return;
                }
            }

            Logger.DangerLine("检测图片失败。");
        }

        private class GroupSettings
        {
            public CommonMessage MessageObj { get; set; }
            public string GroupId { get; set; }
            public List<string> ReceivedString { get; } = new List<string>();
            public Queue<string> PathQueue { get; } = new Queue<string>();
            public Thread Thread { get; set; }
            public Process Process { get; set; }
            public int PandaCount { get; set; }
        }
    }
}