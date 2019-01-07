using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common;
using Daylily.Common.Logging;
using Daylily.Common.Web;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

//using System.Threading;

namespace Daylily.Plugin.Fun
{
    [Name("熊猫斗图")]
    [Author("yf_extension", "sahuang")]
    [Version(2, 0, 1, PluginVersion.Beta)]
    [Help("发现熊猫图时有几率返回一张熊猫图。")]
    class PandaDetectorApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("6dbf918c-0ee4-49c3-abbf-982b8358db31");

        private static readonly ConcurrentDictionary<string, GroupSettings> GroupDic =
            new ConcurrentDictionary<string, GroupSettings>();
#if DEBUG
        private static int _totalCount;
#endif

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.MessageType == MessageType.Private)
                return null;
            var imgList = CoolQCode.GetImageInfo(routeMsg.RawMessage);
            if (imgList == null) return null;

            string groupId = routeMsg.GroupId ?? routeMsg.DiscussId;

            if (!GroupDic.ContainsKey(groupId))
                GroupDic.GetOrAdd(groupId, new GroupSettings
                {
                    GroupId = groupId,
                    routeMsg = routeMsg
                });

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
                    string path = HttpClient.SaveImageFromUrl(item.Url, System.Drawing.Imaging.ImageFormat.Jpeg);
                    GroupDic[groupId].PathQueue.Enqueue(path);
                }
#if DEBUG
                _totalCount++;
#endif
            }

            if (GroupDic[groupId].Task == null || GroupDic[groupId].Task.IsCompleted ||
                GroupDic[groupId].Task.IsCanceled)
            {
                GroupDic[groupId].Task = Task.Run(() => RunDetector(GroupDic[groupId]));
#if DEBUG
                Logger.Info("[" + groupId + "] (熊猫) 共 " + _totalCount);
#endif
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
                    Logger.Exception(ex);
                }
                finally
                {
#if DEBUG
                    _totalCount--;
                    Logger.Info("(熊猫) " + (_totalCount + 1) + " ---> " + _totalCount);
#endif
                }
            }

            if (gSets.PandaCount < 1) return;
#if DEBUG
            Logger.Info("[" + gSets.GroupId + "] (熊猫) " + gSets.PandaCount);
#endif
            for (int i = 0; i < gSets.PandaCount; i++)
            {
                var perc = StaticRandom.NextDouble();
                if (perc >= 0.15)
                    continue;
                Logger.Success("[" + gSets.GroupId + "] (熊猫) 几率: " + perc);

                string resPath = Path.Combine(Domain.PluginPath, "dragon", "resource_panda_send");
                FileInfo[] files = new DirectoryInfo(resPath).GetFiles();
                var cqImg = new FileImage(files[StaticRandom.Next(files.Length)].FullName).ToString();
                SendMessage(gSets.routeMsg.ToSource(cqImg));
            }

            gSets.Clear();
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
                        $"{Path.Combine(Domain.PluginPath, "dragon", "panda-detection.py")} \"{fullPath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            gSets.Process.OutputDataReceived += (sender, e) =>
            {
#if DEBUG
                Logger.Debug(e.Data);
#endif
                if (e.Data != null && e.Data.Trim() != "") gSets.ReceivedString.Add(e.Data);
            };

            gSets.Process.ErrorDataReceived += (sender, e) =>
            {
#if DEBUG
                Logger.Warn(e.Data);
#endif
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

            Logger.Error("检测图片失败。");
        }

        private class GroupSettings
        {
            public CoolQRouteMessage routeMsg { get; set; }
            public string GroupId { get; set; }
            public List<string> ReceivedString { get; } = new List<string>();
            public Queue<string> PathQueue { get; } = new Queue<string>();
            public Task Task { get; set; }
            public Process Process { get; set; }
            public int PandaCount { get; set; }

            public void Clear()
            {
                PandaCount = 0;
                ReceivedString.Clear();
            }
        }
    }
}