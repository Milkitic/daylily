using Daylily.Bot;
using Daylily.Bot.Backend.Plugins;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyPinyin.Core;

namespace Daylily.Plugin.Fun
{
    public class DragonLanguage : ApplicationPlugin
    {
        public override Guid Guid { get; }

        private readonly string[] _dragonMessages = { "wslnm", "nmsl", "nmntys", "fnmdp" };
        private List<string> UserDictionary { get; set; }

        public override void OnInitialized(string[] args)
        {
            UserDictionary = LoadSettings<List<string>>() ?? new List<string>();
        }

        public override RouteMessage OnMessageReceived(ScopeEventArgs scope)
        {
            var routeMsg = (CoolQRouteMessage)scope.RouteMessage;
            var msg = routeMsg.RawMessage;
            if (Detect(msg))
            {
                UserDictionary.Add(msg);
                SaveSettings(UserDictionary);
                return routeMsg.ToSource(msg.Length < 10
                    ? "你龙语了？"
                    : UserDictionary[StaticRandom.Next(UserDictionary.Count)]);
            }

            return null;
        }

        private bool Detect(string msg)
        {
            var compared = _dragonMessages;
            var list = new List<char[]>();
            var word = new StringBuilder();
            foreach (var c in msg)
            {
                if (PinyinHelper.IsChinese(c))
                {
                    DetectWord(list, word);
                    list.Add(DetectPinyin(c));
                }
                else if (c == ' ')
                {
                    DetectWord(list, word);
                }
                else
                {
                    word.Append(c);
                }
            }

            throw new NotImplementedException();
        }

        private static void DetectWord(ICollection<char[]> list, StringBuilder word)
        {
            if (word.Length != 0)
            {
                list.Add(GetChars(word.ToString()));
                word.Clear();
            }
        }

        private static char[] DetectPinyin(char c)
        {
            return new[] { PinyinHelper.GetPinyin(c).FirstOrDefault() };
        }

        private static char[] GetChars(string c)
        {
            return c.Where(k => k != 'a' && k != 'e' && k != 'i' && k != 'o' && k != 'u').ToArray();
        }
    }
}
