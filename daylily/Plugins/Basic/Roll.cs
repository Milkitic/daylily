using System.ComponentModel;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Basic;

[PluginIdentifier("bbcfc459-20b2-483b-89be-d7fe3289010d", "获取随机数")]
[Description("获取一个或多个随机数。")]
public class Roll : BasicPlugin
{
    [CommandHandler("roll")]
    public IResponse RollNumbers(
        [Option("r"), Description("若启用，则使抽取含重复结果。否则结果不包含重复结果。")]
        bool repeat = false,
        [Argument, Description("当参数(m)为无效参数时，此参数(n)为上界(0~n)。否则此参数为下界(n~m)。")]
        int? param1 = null,
        [Argument, Description("此参数(m)为上界(n~m)。")]
        int? param2 = null,
        [Argument, Description("此参数(c)为抽取的数量。")]
        int? count = null)
    {
        var rand = Random.Shared;

        if (param1 == null)
            return Reply(GetRand(rand).ToString());
        if (param2 == null)
            return Reply(GetRand(param1.Value, rand).ToString());
        if (count == null)
            return Reply(GetRand(param1.Value, param2.Value, rand).ToString());
        return Reply(GetRandMessage(param1.Value, param2.Value, repeat, count.Value, rand));
    }

    private static int GetRand(Random random) => random.Next(0, 101);

    private static int GetRand(int uBound, Random random) => random.Next(0, uBound + 1);

    private static int GetRand(int lBound, int uBound, Random random) => random.Next(lBound, uBound + 1);

    private static string GetRandMessage(int lBound, int uBound, bool repeat, int count, Random random)
    {
        uBound += 1;
        if (uBound - lBound > 1000) return "总数只支持1000以内……";
        if (count > 30) return "次数不能大于30……";
        if (count < 0) return "缩不粗化";

        List<int> newList = new List<int>();
        if (repeat || count > uBound - lBound)
        {
            string repMsg = ((count > uBound - lBound) || repeat) ? "（包含重复结果）" : "";
            for (int i = 0; i < count; i++)
            {
                newList.Add(GetRand(lBound, uBound - 1, random));
            }

            newList.Sort();
            return repMsg + string.Join(", ", newList);
        }

        // else
        List<int> list = new List<int>();
        for (int i = lBound; i < uBound; i++)
            list.Add(i);

        for (int i = 0; i < count; i++)
        {
            int index = random.Next(0, list.Count);
            newList.Add(list[index]);
            list.RemoveAt(index);
        }

        newList.Sort();
        return string.Join(", ", newList);
    }
}