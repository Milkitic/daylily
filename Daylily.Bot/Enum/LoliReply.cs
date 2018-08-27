using System;

namespace Daylily.Bot.Enum
{
    public static class LoliReply
    {
        public const string IdNotBound = "你还没有绑定osu ID, 所以我还不知道你是哪位呢… 用setid让我康康你是谁吧吧。";
        public const string IdNotFound = "…根本找不到这个osu ID。要不试试用引号括起来？";

        public const string PrivateOnly = "这个功能，是仅限私聊的。";
        public const string GroupDiscussOnly = "这个功能，是仅限群聊的。";

        public const string RootOnly = "这样子是不可以的…它需要root权限才能启动。";
        public const string AdminOnly = "这样子是不可以的…它需要本群管理员才能启动。";

        public const string ParamError = "命令后面参数的不对哦，请使用 /help [命令] 查看说明。";
        public const string ParamMissing = "请填写参数…请使用 /help [命令] 查看说明。";

        public static string FakeRoot
        {
            get
            {
                string[] exp = { "你没有root权限哦！没有哦！！！" };
                return exp[Random.Next(0, exp.Length)];
            }
        }

        public const string FakeAdmin = "你不是这个群的管理员，所以没有办法使用提权命令。";

        private static readonly Random Random = new Random();
    }
}
