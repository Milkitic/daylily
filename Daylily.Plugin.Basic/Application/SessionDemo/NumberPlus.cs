using Daylily.Bot.Backend;
using Daylily.Bot.Session;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Daylily.Plugin.Basic.Application.SessionDemo
{
    [Name("两数相加")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Stable)]
    [Help("两数相加测试。Session应用的demo。")]
    internal class NumberPlus : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("44ca2fd0-a6dc-48aa-a41b-7b6abfd6e2d4");

        private static readonly ConcurrentDictionary<Session, (string, List<string>)> SessionsList =
            new ConcurrentDictionary<Session, (string, List<string>)>();
        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.RawMessage.Contains("两数相加"))
            {
                if (SessionsList.Count >= 3)
                {
                    return routeMsg.ToSource("可创建的房间已大于限制，请匹配或者等待目前的游戏结束。");
                }

                string roomNum = Guid.NewGuid().ToString().Split('-')[0];
                try
                {
                    using (Session session = new Session(30000, routeMsg.Identity, routeMsg.UserId))
                    {
                        SessionsList.TryAdd(session, (roomNum, new List<string> { routeMsg.UserId }));
                        try
                        {
                            SendMessage(routeMsg.ToSource($"创建了房间，房间号：#{roomNum} 正在等待对手..."));
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            while (dic.Count < 2)
                            {
                                CoolQRouteMessage obj = (CoolQRouteMessage)session.GetMessage();
                                if (dic.Keys.Contains(obj.UserId))
                                    dic[obj.UserId] = obj.RawMessage;
                                else
                                    dic.Add(obj.UserId, obj.RawMessage);
                            }

                            int[] nums;
                            try
                            {
                                nums = dic.Select(k => int.Parse(k.Value)).ToArray();
                            }
                            catch (Exception)
                            {
                                return routeMsg.ToSource("由于一方没有输入正确的数字，无结果。");
                            }
                            string[] users = dic.Select(k => k.Key).ToArray();

                            return routeMsg.ToSource($"{string.Join("与", users)}的结果：{nums.Sum()}");
                        }
                        catch (TimeoutException)
                        {
                            SessionsList.TryRemove(session, out _);
                            return routeMsg.ToSource($"由于长时间不使用，#{roomNum} 房间已关闭");
                        }
                    }
                }
                catch (NotSupportedException)
                {
                    return routeMsg.ToSource("你已在房间中。", true);
                }
            }
            else if (routeMsg.RawMessage.Contains("匹配"))
            {
                var exist = SessionsList.Where(k => k.Value.Item2.Contains(routeMsg.UserId)).ToArray();
                if (exist.Length != 0)
                    return routeMsg.ToSource("你已经创建过房间 #" + exist[0].Value.Item1 + "，等待匹配。", true);
                var list = SessionsList.Where(k => k.Value.Item2.Count < 2).ToList();
                if (list.Count == 0)
                    return routeMsg.ToSource("当前没有空闲房间。");

                int i = StaticRandom.Next(0, list.Count);
                var ok2 = list[i];

                Session session = ok2.Key;
                string roomNum = ok2.Value.Item1;
                List<string> memberList = ok2.Value.Item2;

                session.AddMember(routeMsg.UserId);
                return memberList.Count == 0
                    ? routeMsg.ToSource($"加入房间，房间号：#{roomNum} 正在等待对手……")
                    : routeMsg.ToSource($"匹配成功，房间号：#{roomNum} 对手：{memberList[0]}", true);
            }

            return null;
        }
    }
}
