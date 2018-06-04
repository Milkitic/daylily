using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    public class Roll : AppConstruct
    {
        public override string Name => "获取随机数";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => "获取一个随机数";
        public override string Command => "roll";
        public override AppType AppType => AppType.Command;

        public override void OnLoad(string[] args)
        {
            throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage message)
        {
            var query = message.Parameter.Split(' ');
            if (!int.TryParse(query[0], out _))
            {
                return new CommonMessageResponse(Next().ToString(), message, true);
            }

            switch (query.Length)
            {
                case 1:
                    return new CommonMessageResponse(Next(int.Parse(query[0])).ToString(), message, true);
                case 2:
                    return new CommonMessageResponse(Next(int.Parse(query[0]), int.Parse(query[1])).ToString(), message,
                        true);
                case 3:
                    return new CommonMessageResponse(
                        Next(int.Parse(query[0]), int.Parse(query[1]), int.Parse(query[2])), message, true);
                default:
                    return new CommonMessageResponse(LoliReply.ParamError, message, true);
            }
        }

        private int Next()
        {
            return Rnd.Next(0, 101);
        }

        private int Next(int uBound)
        {
            return Rnd.Next(0, uBound + 1);
        }

        private int Next(int lBound, int uBound)
        {
            return Rnd.Next(lBound, uBound + 1);
        }

        private string Next(int lBound, int uBound, int times)
        {
            if (uBound - lBound > 1000) return ("能不能不要把总数设太多..1000以内");
            if (times > 30) return ("次数不能大于30...");
            if (times < 0) return ("你想怎么样.jpg");
            if (times > uBound - lBound) return ("你这样我没法给结果。");
            List<int> list = new List<int>();
            for (int i = lBound; i <= uBound; i++)
            {
                list.Add(i);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < times; i++)
            {
                int index = Rnd.Next(0, list.Count);
                sb.Append(list[index] + ", ");
                list.RemoveAt(index);
            }

            return sb.ToString().Trim().Trim(',');
        }
    }
}
