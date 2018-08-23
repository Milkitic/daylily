using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common;
using Daylily.CoolQ;

namespace Daylily.Plugin.ShaDiao.Application
{
    [Name("关键词触发")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Alpha)]
    [Help("收到已给的关键词时，根据已给几率返回一张熊猫图。")]
    public class KeywordTrigger : ApplicationPlugin
    {
        private static readonly string PandaDir = Path.Combine(Domain.CurrentDirectory, "panda");
        private static List<TriggerObject> _triggerObjects;
        public KeywordTrigger()
        {
            _triggerObjects = LoadSettings<List<TriggerObject>>("UserDictionary") ?? new List<TriggerObject>
            {
                new TriggerObject(new[] {"我"}, new[] {"me1.jpg"}, 0.5),
                new TriggerObject(new[] {"你"}, new[] {"you1.jpg", "you2.jpg"}, 2),
                new TriggerObject(new[] {"为啥", "为什么", "为毛", "为嘛", "why "}, new[] {"why1.jpg"}, 20),
                new TriggerObject(new[] {"看来", "原来"}, new[] {"kanlai1.jpg", "kanlai2.jpg"}, 30),
                new TriggerObject(new[] {"黄花菜"},
                    new[]
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

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            if (messageObj.Command == "keyedit")
            {
                if (messageObj.FreeArgs.Count == 1)
                    return new CommonMessageResponse(messageObj.FreeArgs[0] + " (KeywordTrigger)", messageObj);
            }
            string msg = messageObj.Message;

            foreach (var item in _triggerObjects)
            {
                if (Trig(msg, item.Words, item.Pictrues, out string img, item.ChancePercent))
                    return new CommonMessageResponse(new FileImage(Path.Combine(PandaDir, img)).ToString(), messageObj);
            }

            return null;
        }

        private static bool Trig(string message, IEnumerable<string> keywords, IReadOnlyList<string> pics,
            out string imgP, double chancePercent = 10)
        {
            var chance = chancePercent / 100d;
            string msg = message.ToLower();
            if (keywords.Any(msg.Contains))
            {
                imgP = pics[Rnd.Next(pics.Count)];
                return Rnd.NextDouble() < chance;
            }

            imgP = null;
            return false;
        }

        public class TriggerObject
        {
            public TriggerObject(IEnumerable<string> words, IReadOnlyList<string> pictrues, double chancePercent)
            {
                Contract.Requires<ArgumentOutOfRangeException>(chancePercent >= 0 && chancePercent <= 100);
                Contract.Requires<ArgumentOutOfRangeException>(words != null);
                Contract.Requires<ArgumentOutOfRangeException>(pictrues != null);
                Words = words;
                Pictrues = pictrues;
                ChancePercent = chancePercent;
            }

            public IEnumerable<string> Words { get; set; }
            public IReadOnlyList<string> Pictrues { get; set; }
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
