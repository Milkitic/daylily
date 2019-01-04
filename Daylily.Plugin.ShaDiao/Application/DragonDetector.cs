using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.RequestUtils;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Daylily.Plugin.ShaDiao.Application
{
    [Name("龙图检测")]
    [Author("yf_extension", "sahuang")]
    [Version(2, 0, 0, PluginVersion.Alpha)]
    [Help("发现龙图时作出回应。", "仅供娱乐。")]
    class DragonDetector : CoolQApplicationPlugin
    {
        private static readonly ConcurrentDictionary<string, GroupSettings> GroupDic = new ConcurrentDictionary<string, GroupSettings>();

        private static int _totalCount;

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            if (routeMsg.MessageType == MessageType.Private)
                return null;
            string groupId = routeMsg.GroupId ?? routeMsg.DiscussId;

            if (!GroupDic.ContainsKey(groupId))
                GroupDic.GetOrAdd(groupId, new GroupSettings
                {
                    GroupId = groupId,
                    routeMsg = routeMsg
                });

            var imgList = CoolQCode.GetImageInfo(routeMsg.RawMessage);
            if (imgList == null) return null;

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
                    var root = WebRequestUtil.GetImageFromUrl(item.Url, item.Md5, item.Extension);
                    GroupDic[groupId].PathQueue.Enqueue(root);
                }

                _totalCount++;
            }

            if (GroupDic[groupId].Task == null || GroupDic[groupId].Task.IsCompleted ||
                GroupDic[groupId].Task.IsCanceled)
            {
                GroupDic[groupId].Task = Task.Run(() => RunDetector(GroupDic[groupId]));
                Logger.Info("[" + groupId + "] (龙图) 共 " + _totalCount);
            }

            return null;
        }

        /// <summary>
        /// 核心识别by sahuang
        /// </summary>
        private static void RunDetector(GroupSettings gSets)
        {
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
                    _totalCount--;
                    Logger.Info("(龙图) " + (_totalCount + 1) + " ---> " + _totalCount);
                }
            }

            if (gSets.DragonCount < 1) return;
            Logger.Info("[" + gSets.GroupId + "] (龙图) " + gSets.DragonCount);
            SendMessage(gSets.routeMsg.ToSource("你龙了?"));
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
                        $"{Path.Combine(Domain.PluginPath, "dragon", "dragon-detection.py")} \"{fullPath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            gSets.Process.OutputDataReceived += (sender, e) =>
            {
                Logger.Debug(e.Data);
                if (e.Data != null && e.Data.Trim() != "") gSets.ReceivedString.Add(e.Data);
            };

            gSets.Process.ErrorDataReceived += (sender, e) =>
            {
                Logger.Debug(e.Data);
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
                    if (status == 1 && confidence > 68)
                        gSets.DragonCount++;
                    return;
                }
            }

            Logger.Error("检测龙图失败。");
        }

        private class GroupSettings
        {
            public CoolQRouteMessage routeMsg { get; set; }
            public string GroupId { get; set; }
            public List<string> ReceivedString { get; } = new List<string>();
            public Queue<string> PathQueue { get; } = new Queue<string>();
            public Task Task { get; set; }
            public Process Process { get; set; }
            public int DragonCount { get; set; }

            public void Clear()
            {
                DragonCount = 0;
                ReceivedString.Clear();
            }
        }
    }
}
