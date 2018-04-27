using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaylilyWeb.Assist;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using DaylilyWeb.Models.CosResponse;

namespace DaylilyWeb.Functions.Applications
{
    public class DragonDetectorAlpha : Application
    {
        HttpApi CQApi = new HttpApi();
        Thread thread;

        private Process proc;
        private List<string> receivedString = new List<string>();
        List<string> pathList = new List<string>();
        int dragonCount = 0;
        private static int totalCount = 0;
        string user, group;

        public override string Execute(string message, string user, string group, bool isRoot, ref bool ifAt)
        {
            //if (group == null) return null;
            if (user != "2241521134") return null;
            this.user = user;
            this.group = group;

            var img_list = CQCode.GetImageInfo(message);
            if (img_list == null)
                return null;

            foreach (var item in img_list)
            {
                if (item.Extension.ToLower() == ".gif")
                    continue;
                if (item.FileInfo.Exists)
                {
                    pathList.Add(item.FileInfo.FullName);
                }
                else
                {
                    WebRequestHelper.GetImageFromUrl(item.Url, item.Md5, item.Extension);
                    pathList.Add(Path.Combine(Environment.CurrentDirectory, "images", item.Md5 + item.Extension));
                }
                totalCount++;
            }
            thread = new Thread(new ParameterizedThreadStart(RunDetector));
            thread.Start(pathList);
            Logger.WriteLine("已经发送了请求,目前队列中共" + totalCount);
            return null;
        }

        /// <summary>
        /// 核心识别by sahuang
        /// </summary>
        private void RunDetector(object pathList)
        {
            try
            {
                var list = (List<string>)pathList;
                foreach (var fullPath in list)
                {
                    Thread.Sleep(6000);
                    totalCount--;
                    continue;
                    if (proc != null)
                    {
                        if (!proc.HasExited) proc.Kill();
                        proc = null;
                    }
                    proc = new Process();
                    proc.StartInfo.FileName = "python3";  // python3 dragon-detection.py "root"
                    proc.StartInfo.Arguments = $"{Path.Combine(Environment.CurrentDirectory, "dragon", "dragon-detection.py")} \"{fullPath}\"";      // 参数  

                    proc.StartInfo.CreateNoWindow = true;
                    //proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    proc.StartInfo.RedirectStandardOutput = true; // 重定向标准输出  
                    proc.StartInfo.RedirectStandardError = true;  // 重定向错误输出  
                    proc.OutputDataReceived += new DataReceivedEventHandler(ProcOutputReceived);
                    proc.ErrorDataReceived += new DataReceivedEventHandler(ProcErrorReceived);

                    Console.WriteLine("正在调用中");
                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    proc.WaitForExit();
                    ProcExited();
                }
                if (dragonCount > 0)
                {
                    CQApi.SetGroupBan(group, user, rnd.Next(1, 100 * dragonCount + 1) * 60);
                    CQApi.SendGroupMessageAsync(group, CQCode.EncodeAt(user) + " 你龙了");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.DangerLine(ex.Message);
            }
        }

        private void ProcOutputReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Trim() == "") return;
            receivedString.Add(e.Data);
            //Console.WriteLine(e.Data);
        }
        private void ProcErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Trim() == "") return;
            receivedString.Add(e.Data);
            //Console.WriteLine(e.Data);
        }
        private void ProcExited()
        {
            int status;
            double confidence;

            //receivedString.RemoveAll(x => x == null);

            if (receivedString.Count == 0) return;
            string line = receivedString[receivedString.Count - 1];
            Console.WriteLine(line);

            var tmp = line.Split(' ');
            status = int.Parse(tmp[0]);
            confidence = double.Parse(tmp[1]);
            if (status == 1 || confidence > 0.75)
            {
                dragonCount++;
            }
            Console.WriteLine("调用结束");
        }
    }
}
