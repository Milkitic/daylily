using System;
using System.Collections.Generic;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;

namespace Daylily.Plugin.Core
{
    [Name("获取随机数")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("获取一个或多个随机数。")]
    [Command("roll")]
    public class Roll : CommandPlugin
    {
        [Arg("r", IsSwitch = true, Default = false)]
        [Help("若启用，则使抽取含重复结果。否则结果不包含重复结果。")]
        public bool Repeat { get; set; }
        [FreeArg]
        [Help("当参数(m)为无效参数时，此参数(n)为上界(0~n)。否则此参数为下界(n~m)。")]
        public string Param1 { get; set; }
        [FreeArg]
        [Help("此参数(m)为上界(n~m)。")]
        public string Param2 { get; set; }
        [FreeArg]
        [Help("此参数(c)为抽取的数量。")]
        public string Count { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            bool isParam1 = int.TryParse(Param1, out int param1);
            bool isParam2 = int.TryParse(Param2, out int param2);
            bool isCNum = int.TryParse(Count, out int count);
            if (!isParam1)
                return new CommonMessageResponse(GetRand().ToString(), messageObj, true);
            if (!isParam2)
                return new CommonMessageResponse(GetRand(param1).ToString(), messageObj, true);
            if (!isCNum)
                return new CommonMessageResponse(GetRand(param1, param2).ToString(), messageObj, true);
            return new CommonMessageResponse(GetRandMessage(param1, param2, count), messageObj, true);
        }

        private static int GetRand() => Rnd.Next(0, 101);

        private static int GetRand(int uBound) => Rnd.Next(0, uBound + 1);

        private static int GetRand(int lBound, int uBound) => Rnd.Next(lBound, uBound + 1);

        private string GetRandMessage(int lBound, int uBound, int count)
        {
            uBound = uBound + 1;
            if (uBound - lBound > 1000) return "总数只支持1000以内……";
            if (count > 30) return "次数不能大于30……";
            if (count < 0) return "缩不粗化";

            List<int> newList = new List<int>();
            if (Repeat || count > uBound - lBound)
            {
                string repMsg = ((count > uBound - lBound) && !Repeat) ? "（可能包含重复结果）" : "";
                for (int i = 0; i < count; i++)
                {
                    newList.Add(GetRand(lBound, uBound - 1));
                }

                newList.Sort();
                return repMsg + string.Join(',', newList);
            }

            // else
            List<int> list = new List<int>();
            for (int i = lBound; i < uBound; i++)
                list.Add(i);

            for (int i = 0; i < count; i++)
            {
                int index = Rnd.Next(0, list.Count);
                newList.Add(list[index]);
                list.RemoveAt(index);
            }

            newList.Sort();
            return string.Join(',', newList);
        }
    }

    [Name("复读")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("复读")]
    public class Repeat : ApplicationPlugin
    {

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            return new CommonMessageResponse(messageObj.Message, messageObj);
        }
    }
}
