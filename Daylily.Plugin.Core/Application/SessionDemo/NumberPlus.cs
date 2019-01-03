using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Bot.Sessions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Daylily.Plugin.Core.Application.SessionDemo
{
    [Name("两数相加")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("两数相加测试。Session应用的demo。")]
    internal class NumberPlus : ApplicationPlugin
    {
        private static readonly ConcurrentDictionary<Session, (string, List<string>)> SessionsList =
            new ConcurrentDictionary<Session, (string, List<string>)>();
        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            if (messageObj.RawMessage.Contains("两数相加"))
            {
                if (SessionsList.Count >= 3)
                {
                    return new CommonMessageResponse("可创建的房间已大于限制，请匹配或者等待目前的游戏结束。", messageObj);
                }

                string roomNum = Guid.NewGuid().ToString().Split('-')[0];
                try
                {
                    using (Session session = new Session(30000, messageObj.CqIdentity, messageObj.UserId))
                    {
                        SessionsList.TryAdd(session, (roomNum, new List<string> { messageObj.UserId }));
                        try
                        {
                            SendMessage(new CommonMessageResponse($"创建了房间，房间号：#{roomNum} 正在等待对手...", messageObj));
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            while (dic.Count < 2)
                            {
                                CommonMessage obj = session.GetMessage();
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
                                return new CommonMessageResponse("由于一方没有输入正确的数字，无结果。", messageObj);
                            }
                            string[] users = dic.Select(k => k.Key).ToArray();

                            return new CommonMessageResponse($"{string.Join("与", users)}的结果：{nums.Sum()}", messageObj);
                        }
                        catch (TimeoutException)
                        {
                            SessionsList.TryRemove(session, out _);
                            return new CommonMessageResponse($"由于长时间不使用，#{roomNum} 房间已关闭", messageObj);
                        }
                    }
                }
                catch (NotSupportedException)
                {
                    return new CommonMessageResponse("你已在房间中。", messageObj, true);
                }
            }
            else if (messageObj.RawMessage.Contains("匹配"))
            {
                var exist = SessionsList.Where(k => k.Value.Item2.Contains(messageObj.UserId)).ToArray();
                if (exist.Length != 0)
                    return new CommonMessageResponse("你已经创建过房间 #" + exist[0].Value.Item1 + "，等待匹配。", messageObj, true);
                var list = SessionsList.Where(k => k.Value.Item2.Count < 2).ToList();
                if (list.Count == 0)
                    return new CommonMessageResponse("当前没有空闲房间。", messageObj);

                int i = StaticRandom.Next(0, list.Count);
                var ok2 = list[i];

                Session session = ok2.Key;
                string roomNum = ok2.Value.Item1;
                List<string> memberList = ok2.Value.Item2;

                session.AddMember(messageObj.UserId);
                return memberList.Count == 0
                    ? new CommonMessageResponse($"加入房间，房间号：#{roomNum} 正在等待对手……", messageObj)
                    : new CommonMessageResponse($"匹配成功，房间号：#{roomNum} 对手：{memberList[0]}", messageObj, true);
            }

            return null;
        }
    }
}
