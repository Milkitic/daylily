using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Backend.Plugins;
using Daylily.Bot.Message;
using Daylily.Common.Collections;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TinyPinyin.Core;

namespace Daylily.Plugin.Fun
{
    [Name("龙TimeNo色")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Stable)]
    [Help("龙语识别。")]
    public class DragonLanguageApp : CoolQApplicationPlugin
    {
        public override Guid Guid { get; } = new Guid("33cc804f-1704-4e34-a958-85bd9d1069e1");

        private static readonly string[] Templates = { "nmsl", "wslnm", "nmntys", "fnmdp", "rsndm" };
        private static readonly string[] Filter = { "你妈", "妈死", "日死", "你吗", "nm", "nima", "ni ma" };
        private ConcurrentDictionary<string, List<UserExpression>> UserDictionary { get; set; }
        private int Count => UserDictionary.Sum(k => k.Value.Count);
        public override void OnInitialized(string[] args)
        {
            UserDictionary = LoadSettings<ConcurrentDictionary<string, List<UserExpression>>>() ??
                             new ConcurrentDictionary<string, List<UserExpression>>();
        }

        public class UserExpression
        {
            public UserExpression(string expression)
            {
                Expression = expression;
            }

            public int Times { get; set; } = 1;
            public string Expression { get; set; }
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            var msg = routeMsg.RawMessage;
            if (!Detect(msg, out var tuples)) return null;
            foreach ((string origin, string mine) in tuples)
            {
                if (Filter.Any(k => mine.Contains(k)))
                    continue;
                if (!UserDictionary.ContainsKey(origin))
                    UserDictionary.TryAdd(origin, new List<UserExpression>());
                var o = UserDictionary[origin].FirstOrDefault(k => k.Expression == mine);
                if (o == null)
                    UserDictionary[origin].Add(new UserExpression(mine));
                else o.Times++;
            }

            SaveSettings(UserDictionary);
            if (Count >= 30)
            {
                var sayRate = StaticRandom.NextDouble();
                var mark = StaticRandom.NextDouble() <= 0.3 ? "?" : "";
                if (sayRate <= 0.3)
                {
                    var keys = UserDictionary.Keys.ToList();
                    var key = keys[StaticRandom.Next(keys.Count)];
                    var list = UserDictionary[key];
                    
                    if (key != Templates[0])
                    {
                        var nextRate = StaticRandom.NextDouble();
                        if (nextRate <= 0.2 && keys.Contains(Templates[0]))
                        {
                            var list2 = UserDictionary[Templates[0]];
                            routeMsg.ToSource(list[StaticRandom.Next(list.Count)].Expression + ", " +
                                              list2[StaticRandom.Next(list2.Count)].Expression + mark);
                        }
                        else
                            return routeMsg.ToSource(list[StaticRandom.Next(list.Count)].Expression + mark);
                    }
                    else
                        return routeMsg.ToSource(list[StaticRandom.Next(list.Count)].Expression + mark);
                }


            }

            Thread.Sleep(StaticRandom.Next(0, 7000));
            return null;
        }

        private bool Detect(string msg, out (string origin, string mine)[] info)
        {
            var copy = Templates.ToArray();
            var dr = new DetectedResults(msg);
            var word = new StringBuilder();
            int index = 0;
            foreach (var c in msg)
            {
                if (PinyinHelper.IsChinese(c))
                {
                    DetectWord(index, dr, word);
                    dr.Add(index, 1, false, DetectPinyin(c));
                }
                else if (c > 128 || !(c >= 65 && c <= 90 || c >= 97 && c <= 122))
                {
                    DetectWord(index, dr, word);
                }
                else
                {
                    word.Append(c);
                }

                index++;
            }

            DetectWord(index, dr, word);


            if (!dr.Judge())
            {
                info = null;
                return false;
            }
            info = dr.Patterned.ToArray();
            return true;
        }


        private void DetectWord(int index, DetectedResults dr, StringBuilder word)
        {
            if (word.Length == 0) return;
            char[] cs = GetChars(word.ToString(), Templates);
            var chars = new Chars(index - word.Length, word.Length, word.Length != 1);
            chars.Add(cs[0]);
            for (int i = 2; i <= cs.Length; i++)
            {
                var array = cs.Take(i).ToArray();
                if (SequenceContains(new string(array), Templates))
                {
                    chars.Add(array);
                }
                else
                {
                    break;
                }
            }
            dr.Add(chars);
            word.Clear();
        }

        private static char[] DetectPinyin(char c) => new[]
        {
            ToLower(PinyinHelper.GetPinyin(c).FirstOrDefault())
        };

        private static char ToLower(char c)
        {
            if (c >= 65 && c <= 90)
                return (char)(c + (char)32);
            return c;
        }

        private static string ToLower(string s)
        {
            return s?.ToLower();
        }

        private static char[] GetChars(string s, IEnumerable<string> array) =>
            s.Take(1)
                .Select(ToLower)
                .Concat(
                    s.Skip(1)
                        .Where(k => SequenceContains(k, array))
                        .Select(ToLower)
                ).ToArray();

        private static bool SequenceContains(char? c, IEnumerable<string> array) =>
            c != null && array.Any(g => g.Contains(ToLower(c.Value)));
        private static bool SequenceContains(string s, IEnumerable<string> array) =>
            s != null && array.Any(g => g.Contains(ToLower(s)));

        private class DetectedResults
        {
            private readonly string _msg;

            public DetectedResults(string msg)
            {
                _msg = msg;
            }

            public List<Chars> List { get; set; } = new List<Chars>();
            private char[] GetSingleChars()
            {
                List<char> chars = new List<char>();
                var str = List.Select(k => new string(k.Last));
                foreach (var s in str)
                    chars.AddRange(s.ToArray());
                return chars.ToArray();
            }

            public List<(string origin, string mine)> Patterned { get; private set; }
            public bool Judge()
            {
                Patterned?.Clear();
                Patterned = new List<(string str, string mine)>();
                if (!List.Any(k => k.IsPossible))
                    return false;
                bool judge = false;
                var singleChars = GetSingleChars();
                List<(int startIndex, int length, string str)[]> allPossibility = null;

                foreach (var template in Templates)
                {
                    if (!SequentialContainsAll(singleChars, template.ToArray()))
                        continue;
                    if (allPossibility == null)
                        allPossibility = GetAllPossibility();

                    foreach (var possibility in allPossibility)
                    {
                        (int startIndex, int length, string str)[]
                            singledPoss = possibility.Select(k => k.str.ToCharArray()
                                .Select(g => (k.startIndex, k.length, g.ToString())))
                            .SelectMany(valueTuples => valueTuples)
                            .ToArray();

                        for (int i = 0; i <= singledPoss.Length - template.Length; i++)
                        {

                            var filtered = new List<(int startIndex, int length, string str)>();
                            int offset = 0;
                            for (int j = i; j < i + template.Length; j++)
                            {
                                var pos = singledPoss[j - offset];
                                filtered.Add((pos.startIndex, pos.length, pos.str));
                                j += pos.str.Length - 1;
                                offset += pos.str.Length - 1;
                            }

                            //var filtered = possibility..Skip(i).Take(template.Length).ToArray();
                            var combined = (filtered.First().startIndex,
                                filtered.Last().startIndex + filtered.Last().length,
                                string.Join("", filtered.Select(k => k.str)));
                            var index = combined.Item3.IndexOf(template, StringComparison.Ordinal);
                            if (index == -1) continue;
                            var mine = _msg.Substring(combined.startIndex, combined.Item2 - combined.startIndex);
                            if (!Patterned.Exists(k => k.mine == mine))
                                Patterned.Add((template, mine));
                            judge = true;
                        }

                    }
                }

                return judge;

            }

            private List<(int startIndex, int length, string str)[]> GetAllPossibility()
            {
                var maxLengths = List.Select(k => k.MaxLength).ToArray();
                var nowIndexes = List.Select(k => 0).ToArray();
                var allPossibility = new List<(int startIndex, int length, string str)[]>();
                do
                {

                    var sb = new (int startIndex, int length, string word)[List.Count];

                    for (var i = 0; i < List.Count; i++)
                    {
                        var chars = List[i];
                        var chars1 = chars[nowIndexes[i]];
                        sb[i] = (List[i].Index, List[i].ActualLength, new string(chars1));
                    }

                    allPossibility.Add(sb);

                } while (NextIndex(nowIndexes, maxLengths));
                return allPossibility;
            }

            private bool NextIndex(int[] nowIndexes, int[] maxLengths)
            {
                nowIndexes[0]++;
                return !ValidateOutOfRange(nowIndexes, maxLengths);
            }

            private static bool ValidateOutOfRange(int[] nowIndexes, int[] maxLengths, int currentIndex = 0)
            {
                bool b = false;
                if (nowIndexes[currentIndex] > maxLengths[currentIndex] - 1)
                {
                    if (currentIndex == nowIndexes.Length - 1)
                    {
                        return true;
                    }

                    nowIndexes[currentIndex] = 0;
                    nowIndexes[currentIndex + 1]++;

                    b = ValidateOutOfRange(nowIndexes, maxLengths, currentIndex + 1);
                }

                return b;
            }

            private bool SequentialContainsAll(IEnumerable<char> source, IReadOnlyList<char> comparision)
            {
                int j = 0;
                foreach (var t in source)
                {
                    if (t == comparision[j])
                        j++;
                    if (j >= comparision.Count)
                        return true;
                }

                return false;
            }

            public void Add(Chars chars)
            {
                List.Add(chars);
            }

            public void Add(int index, int actualLength, bool isWord, char[] chars)
            {
                Add(new Chars(index, actualLength, isWord, chars));
            }

            public override string ToString()
            {
                return $"{{{string.Join(", ", List.Select(k => k.ToString()))}}}";
            }
        }

        private class Chars
        {
            public int Index { get; }
            public int MaxLength => Last.Length;
            public int ActualLength { get; set; }
            public bool IsWord { get; }
            public char[] this[int val] => Characters[val];
            public bool IsPossible => Characters.Any() && SequenceContains(Characters[0][0], Templates);

            public char[] First => Characters.First();
            public char[] Last => Characters.Last();

            public Chars(int index, int actualLength, bool isWord)
            {
                Index = index;
                ActualLength = actualLength;
                IsWord = isWord;
            }

            public Chars(int index, int actualLength, bool isWord, params char[] chars) : this(index, actualLength, isWord)
            {
                Characters.Add(chars);
            }

            public void Add(params char[] chars)
            {
                Characters.Add(chars);
            }

            public Chars Append(params char[] chars)
            {
                Characters.Add(chars);
                return this;
            }

            public List<char[]> Characters { get; } = new List<char[]>();
            public override string ToString()
            {

                return (IsWord ? "[" : "") +
                       $"{string.Join("|", Characters.Select(k => new string(k)))}" +
                       (IsWord ? "]" : "") +
                       (IsWord ? $"({Index}->{Index + ActualLength - 1})" : "");
            }
        }
    }
}
