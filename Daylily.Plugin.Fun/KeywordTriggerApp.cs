using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Command;
using Daylily.Bot.Messaging;
using Daylily.Common;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using Daylily.CoolQ.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Daylily.Plugin.Fun
{
    [Name("关键词触发")]
    [Author("yf_extension")]
    [Version(2, 0, 2, PluginVersion.Alpha)]
    [Help("收到已给的关键词时，根据已给几率返回一张熊猫图。")]
    public class KeywordTriggerApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("1abf7c43-bafb-4536-a2be-e9aff8a2fc51");

        private static readonly string PandaDir = Path.Combine(Domain.ResourcePath, "panda");
        private static List<TriggerObject> _triggerObjects;

        public override void OnInitialized(StartupConfig startup)
        {
            _triggerObjects = LoadSettings<List<TriggerObject>>("UserDictionary") ?? new List<TriggerObject>
            {
                new TriggerObject(new List<string> {"我"},
                    new List<string>
                    {
                        "me1.jpg"
                    }, 0.5),
                new TriggerObject(new List<string> {"你"},
                    new List<string>
                    {
                        "you1.jpg", "you2.jpg"
                    }, 2),
                new TriggerObject(new List<string> {"为啥", "为什么", "为毛", "为嘛", "why "},
                    new List<string>
                    {
                        "why1.jpg"
                    }, 20),
                new TriggerObject(new List<string> {"看来", "原来"},
                    new List<string>
                    {
                        "kanlai1.jpg", "kanlai2.jpg"
                    }, 30),
                new TriggerObject(new List<string> {"黄花菜"},
                    new List<string>
                    {
                        "sb1.jpg", "sb2.jpg", "sb3.jpg", "sb4.jpg", "sb5.jpg", "sb6.jpg", "sb7.jpg", "sb8.jpg",
                        "sb9.jpg"
                    }, 50)
            };

            // 概率从小到大排序，更科学
            _triggerObjects.Sort(new TriggerComparer());
            _triggerObjects.RemoveAll(p => p == null);
            SaveSettings(_triggerObjects, "UserDictionary");
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            string fullCmd = routeMsg.FullCommand ?? routeMsg.RawMessage;
            var ca = new CommandAnalyzer<StreamParamDivider>();
            var cmd = ca.Analyze(fullCmd);

            if (cmd.CommandName == "keyword" && routeMsg.CurrentAuthority == Authority.Root)
            {
                if (cmd.Switches.Contains("key-list"))
                {
                    var list = _triggerObjects.Select(k => (string.Join(",", k.Words), k.ChancePercent, k.Pictures.Count));
                    var list2 = list.Select(k => $"{k.Item1}: \r\n  {k.ChancePercent}% {k.Count} Pics");
                    return routeMsg.ToSource(string.Join("\r\n", list2));
                }

                if (cmd.Args.ContainsKey("key-edit"))
                {
                    var arg = cmd.Args["key-edit"];
                    var triggerObject = _triggerObjects.FirstOrDefault(k => k.Words.Contains(arg));
                    if (triggerObject == null)
                    {
                        return routeMsg.ToSource("不存在该关键词（组）哦");
                    }

                    if (cmd.Args.ContainsKey("addkeys"))
                    {
                        var value = cmd.Args["addkeys"];
                        var values = value.Split('|').Where(k => !string.IsNullOrEmpty(k)).ToArray();
                        if (!values.Any())
                        {
                            return routeMsg.ToSource("请填写触发关键词");
                        }

                        foreach (var val in values)
                        {
                            if (!triggerObject.Words.Contains(val))
                            {
                                triggerObject.Words.Add(val);
                            }
                        }

                        return SaveAndReply(routeMsg);

                    }

                    if (cmd.Args.ContainsKey("removekeys"))
                    {
                        var value = cmd.Args["removekeys"];
                        var values = value.Split('|').Where(k => !string.IsNullOrEmpty(k)).ToArray();
                        if (!values.Any())
                        {
                            return routeMsg.ToSource("请填写触发关键词");
                        }

                        foreach (var val in values)
                        {
                            if (triggerObject.Words.Contains(val))
                            {
                                triggerObject.Words.Remove(val);
                            }
                        }

                        return SaveAndReply(routeMsg);
                    }

                    if (cmd.Switches.Contains("addpics"))
                    {
                        throw new NotImplementedException();
                        return SaveAndReply(routeMsg);
                    }

                    if (cmd.Args.ContainsKey("removepics"))
                    {
                        var value = cmd.Args["removepics"];
                        var values = value.Split('|').Where(k => !string.IsNullOrEmpty(k)).ToArray();
                        if (!values.Any())
                        {
                            return routeMsg.ToSource("请填写需要删除的图片");
                        }

                        foreach (var val in values)
                        {
                            if (triggerObject.Pictures.Contains(val))
                            {
                                triggerObject.Pictures.Remove(val);
                            }
                        }

                        return SaveAndReply(routeMsg);
                    }

                    return routeMsg.ToSource("请执行-addkeys, -removekeys, -addpics或-removepics操作");
                }

                if (cmd.Args.ContainsKey("key-detail"))
                {
                    var arg = cmd.Args["key-detail"];
                    var triggerObject = _triggerObjects.FirstOrDefault(k => k.Words.Contains(arg));
                    if (triggerObject == null)
                    {
                        return routeMsg.ToSource("不存在该关键词（组）哦");
                    }

                    return routeMsg.ToSource(string.Join(", ", triggerObject.Pictures));
                }

                if (cmd.Args.ContainsKey("key-del"))
                {
                    var arg = cmd.Args["key-del"];
                    var triggerObject = _triggerObjects.FirstOrDefault(k => k.Words.Contains(arg));
                    if (triggerObject == null)
                    {
                        return routeMsg.ToSource("不存在该关键词（组）哦");
                    }

                    using (Bot.Session.Session session = new Bot.Session.Session(10000, routeMsg.CoolQIdentity, routeMsg.UserId))
                    {

                        SendMessage(routeMsg.ToSource(
                                $"即将删除：{string.Join(", ", triggerObject.Words)} 一组触发，确定请回复 \"确定\""
                            )
                        );
                        if (session.TryGetMessage(out var msgObj))
                        {
                            if (msgObj.RawMessage == "确定")
                            {
                                return SaveAndReply(routeMsg);
                            }
                            else
                                return routeMsg.ToSource("操作已取消。");
                        }

                    }
                }
                return null;
            }

            string msg = routeMsg.RawMessage;

            foreach (var item in _triggerObjects)
            {
                if (Trig(msg, item.Words, item.Pictures, out string img, item.ChancePercent))
                    return routeMsg.ToSource(new FileImage(Path.Combine(PandaDir, img)).ToString());
            }

            return null;
        }

        private CoolQRouteMessage SaveAndReply(CoolQRouteMessage routeMsg)
        {
            SaveUserDictionary();
            return routeMsg.ToSource("操作已成功完成");
        }

        private void SaveUserDictionary()
        {
            SaveSettings(_triggerObjects, "UserDictionary");
        }

        private static bool Trig(string message, IEnumerable<string> keywords, IReadOnlyList<string> pics,
            out string imgP, double chancePercent = 10)
        {
            var chance = chancePercent / 100d;
            string msg = message.ToLower();
            if (keywords.Any(msg.Contains))
            {
                imgP = pics[StaticRandom.Next(pics.Count)];
                return StaticRandom.NextDouble() < chance;
            }

            imgP = null;
            return false;
        }

        public class TriggerObject
        {
            public TriggerObject(List<string> words, List<string> pictures, double chancePercent)
            {
                Contract.Requires<ArgumentOutOfRangeException>(chancePercent >= 0 && chancePercent <= 100);
                Contract.Requires<ArgumentOutOfRangeException>(words != null);
                Contract.Requires<ArgumentOutOfRangeException>(pictures != null);
                Words = words;
                Pictures = pictures;
                ChancePercent = chancePercent;
            }

            [JsonProperty("keys")]
            public List<string> Words { get; set; }
            [JsonProperty("values")]
            public List<string> Pictures { get; set; }
            [JsonProperty("chance")]
            public double ChancePercent { get; set; }
        }

        private class TriggerComparer : IComparer<TriggerObject>
        {
            public int Compare(TriggerObject obj1, TriggerObject obj2)
            {
                if (obj1 == null && obj2 == null)
                    return 0;
                if (obj1 != null && obj2 == null)
                    return 1;
                if (obj1 == null && obj2 != null)
                    return -1;
                return obj1.ChancePercent >= obj2.ChancePercent ? 1 : -1;
            }
        }
    }
}
