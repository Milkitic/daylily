using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Common.Models
{
    public static class LoliReply
    {
        public const string IdNotBound = "你还没有绑定osu ID, 所以我还不知道你是哪位呢.. 用setid + 用户名绑定吧. 如果绑不上, 那我也没有办法了呢owo.";
        public const string IdNotFound = "...根本找不到这个osu ID. 不要为难我好伐";

        public const string PrivateOnly = "私聊了解一下?. owo";
        public const string GroupDiscussOnly = "群聊了解一下?. owo";

        public const string RootOnly = "这样子的话, 是不可以的.. 不过你可以试试再命令前加上root前缀.";
        public const string AdminOnly = "这样子的话, 是不可以的.. 不过你可以试试再命令前加上sudo前缀.";

        public const string ParamError = "命令后面的不正确!! 请使用 /help 查看说明。";
        public const string ParamMissing = "请填写参数..请填写参数. 请填写参数! 因为很重要所以要说三遍";

        public static string FakeRoot
        {
            get
            {
                string[] exp = { "你不是超级管理员! 我不认识你.", "不要冒充我的主人!" };
                return exp[Random.Next(0, exp.Length)];
            }
        }

        public const string FakeAdmin = "你不是这个群的管理员. 人家可不是小孩子可以被你骗!";

        private static readonly Random Random = new Random();
    }
}
