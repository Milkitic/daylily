using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.RequestUtils;
using Daylily.CoolQ;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

//using System.Threading;

namespace Daylily.Plugin.ShaDiao.Application
{
    [Name("熊猫斗图")]
    [Author("yf_extension", "sahuang")]
    [Version(0, 0, 1, PluginVersion.Beta)]
    [Help("发现熊猫图时有几率返回一张熊猫图。")]
    class PandaDetector : ApplicationPlugin
    {
        private static readonly ConcurrentDictionary<string, GroupSettings> GroupDic = new ConcurrentDictionary<string, GroupSettings>();
#if DEBUG
        private static int _totalCount;
#endif

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            if (messageObj.MessageType == MessageType.Private)
                return null;
            var imgList = CqCode.GetImageInfo(messageObj.RawMessage);
            if (imgList == null) return null;

            string groupId = messageObj.GroupId ?? messageObj.DiscussId;

            if (!GroupDic.ContainsKey(groupId))
                GroupDic.GetOrAdd(groupId, new GroupSettings
                {
                    GroupId = groupId,
                    MessageObj = messageObj
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
                    string path = HttpClientUtil.SaveImageFromUrl(item.Url, System.Drawing.Imaging.ImageFormat.Jpeg);
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
                var perc = Rnd.NextDouble();
                if (perc >= 0.15)
                    continue;
                Logger.Success("[" + gSets.GroupId + "] (熊猫) 几率: " + perc);

                string resPath = Path.Combine(Domain.PluginPath, "dragon", "resource_panda_send");
                FileInfo[] files = new DirectoryInfo(resPath).GetFiles();
                var cqImg = new FileImage(files[Rnd.Next(files.Length)].FullName).ToString();
                SendMessage(new CommonMessageResponse(cqImg, gSets.MessageObj));
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
            public CommonMessage MessageObj { get; set; }
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