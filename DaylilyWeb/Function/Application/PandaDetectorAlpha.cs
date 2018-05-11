using DaylilyWeb.Assist;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaylilyWeb.Function.Application
{
    public class PandaDetectorAlpha : Application
    {
        HttpApi CQApi = new HttpApi();
        Thread thread;
        static Random rnd = new Random();
        static Random rnd2 = new Random();
        private Process proc;
        private List<string> receivedString = new List<string>();
        List<string> pathList = new List<string>();
        int pandaCount = 0;
        private static int totalCount = 0;
        string user, group;
        long messageId;

        public override string Execute(string message, string user, string group, PermissionLevel currentLevel, ref bool ifAt, long messageId)
        {
            if (group == "133605766")
                if (DateTime.Now.Hour < 22 && DateTime.Now.Hour > 6)
                    return null;

            //if (user != "2241521134") return null;
            this.user = user;
            this.group = group;
            this.messageId = messageId;

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
            Logger.WarningLine("已经发送了请求,目前队列中共" + totalCount);
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
                    if (proc != null)
                    {
                        if (!proc.HasExited) proc.Kill();
                        proc = null;
                    }
                    proc = new Process();
                    proc.StartInfo.FileName = "python3";  // python3 dragon-detection.py "root"
                    proc.StartInfo.Arguments = $"{Path.Combine(Environment.CurrentDirectory, "dragon", "panda-detection.py")} \"{fullPath}\"";      // 参数  

                    proc.StartInfo.CreateNoWindow = true;
                    //proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    proc.StartInfo.RedirectStandardOutput = true; // 重定向标准输出  
                    proc.StartInfo.RedirectStandardError = true;  // 重定向错误输出  
                    proc.OutputDataReceived += new DataReceivedEventHandler(ProcOutputReceived);
                    proc.ErrorDataReceived += new DataReceivedEventHandler(ProcErrorReceived);

                    Console.WriteLine("(熊猫)正在调用中");
                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    proc.WaitForExit();
                    ProcExited();
                }
                catch (Exception ex)
                {
                    Logger.DangerLine(ex.Message);
                }
                finally
                {
                    totalCount--;
                }
            }

            if (pandaCount > 0)
            {
                var perc2 = rnd.NextDouble();
                if (perc2 < 0.5)
                {
                    DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "dragon", "resource_panda_send"));
                    var files = di.GetFiles();

                    if (group != null) CQApi.SendGroupMessage(group, CQCode.EncodeFileToBase64(files[rnd.Next(files.Length)].FullName));
                    else CQApi.SendPrivateMessage(user, CQCode.EncodeFileToBase64(files[rnd.Next(files.Length)].FullName));
                    return;
                }
                else
                {
                    Logger.WarningLine("几率不够，没有触发：" + perc2);
                }

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
            Logger.WarningLine(line);

            var tmp = line.Split(' ');
            status = int.Parse(tmp[0]);
            confidence = double.Parse(tmp[1]);
            if (status == 1 && confidence > 50)
            {
                //Logger.WarningLine(confidence.ToString());
                pandaCount++;
            }
            Console.WriteLine("(熊猫)调用结束");
        }
    }
}
