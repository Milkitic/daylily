using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daylily.Plugin.Command
{
    public class Roll : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            var query = message.Parameter.Split(' ');
            if (!int.TryParse(query[0], out int a))
            {
                return new CommonMessageResponse(Next().ToString(), message, true);
            }
            else if (query.Length == 1)
            {
                return new CommonMessageResponse(Next(int.Parse(query[0])).ToString(), message, true);
            }
            else if (query.Length == 2)
            {
                return new CommonMessageResponse(Next(int.Parse(query[0]), int.Parse(query[1])).ToString(), message, true);
            }
            else if (query.Length == 3)
            {
                return new CommonMessageResponse(Next(int.Parse(query[0]), int.Parse(query[1]), int.Parse(query[2])), message, true);
            }
            else
                return new CommonMessageResponse("参数不对...", message, true);
        }

        private int Next()
        {
            return rnd.Next(0, 101);
        }

        private int Next(int uBound)
        {
            return rnd.Next(0, uBound + 1);
        }

        private int Next(int lBound, int uBound)
        {
            return rnd.Next(lBound, uBound + 1);
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
                int index = rnd.Next(0, list.Count);
                sb.Append(list[index] + ", ");
                list.RemoveAt(index);
            }

            return sb.ToString().Trim().Trim(',');
        }
    }
}
