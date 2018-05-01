using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daylily.Plugin.Command
{
    public class Roll : Application
    {
        public override string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt)
        {
            ifAt = true;

            var query = @params.Split(' ');
            if (!int.TryParse(query[0], out int a))
            {
                return Next().ToString();
            }
            else if (query.Length == 1)
            {
                return Next(int.Parse(query[0])).ToString();
            }
            else if (query.Length == 2)
            {
                return Next(int.Parse(query[0]), int.Parse(query[1])).ToString();
            }
            else if (query.Length == 3)
            {
                return Next(int.Parse(query[0]), int.Parse(query[1]), int.Parse(query[2]));
            }
            else throw new ArgumentException();
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
            if (uBound - lBound > 1000) throw new ArgumentException("能不能不要把总数设太多，我主人这垃圾算法不够用");
            if (times > 30) throw new ArgumentException("次数不能大于30啊，不然怕怀疑是刷屏会被金特的");
            if (times < 0) throw new ArgumentException("你想怎么样.jpg");
            if (times > uBound - lBound) throw new ArgumentException("你这样我没法给结果。");
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
