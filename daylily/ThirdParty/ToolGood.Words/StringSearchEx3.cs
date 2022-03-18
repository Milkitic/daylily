using System.IO;

namespace daylily.ThirdParty.ToolGood.Words
{
    /// <summary>
    /// 文本搜索（指针版，速度更快） ，如果关键字太多(5W以上)，建议使用 StringSearchEx
    /// 性能从小到大  StringSearch &lt; StringSearchEx &lt; StringSearchEx2 &lt; StringSearchEx3
    /// </summary>
    public class StringSearchEx3 : BaseSearchEx2
    {
        protected Int32[] _guides2;
        protected Int32[] _guidesLength;

        protected internal override void Load(BinaryReader br)
        {
            base.Load(br);
            _guides2 = new int[_guides.Length];
            _guidesLength = new int[_guides.Length];
            for (int i = 0; i < _guides2.Length; i++)
            {
                _guides2[i] = _guides[i][0];
                _guidesLength[i] = _keywords[_guides2[i]].Length;
            }
        }

        public override void SetKeywords(ICollection<string> keywords)
        {
            base.SetKeywords(keywords);
            _guides2 = new int[_guides.Length];
            _guidesLength = new int[_guides.Length];
            for (int i = 0; i < _guides2.Length; i++)
            {
                _guides2[i] = _guides[i][0];
                _guidesLength[i] = _keywords[_guides2[i]].Length;
            }
        }

        #region 查找 替换 查找第一个关键字 判断是否包含关键字

        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public List<string> FindAll(string text)
        {
            var root = new List<string>();

            var spNext = new Span<int>(_next);
            var spCheck = new Span<int>(_check);
            var spKey = new Span<int>(_key);
            var spDict = new Span<int>(_dict);
            var spText = text.AsSpan();

            var p = 0;

            foreach (var c in spText)
            {
                var t = spDict[c];
                if (t == 0)
                {
                    p = 0;
                    continue;
                }

                var next = spNext[p] + t;
                bool find = spKey[next] == t;
                if (!find && p != 0)
                {
                    p = 0;
                    next = spNext[0] + t;
                    find = spKey[next] == t;
                }

                if (!find) continue;

                var index = spCheck[next];
                if (index > 0)
                {
                    for (var i = 0; i < _guides[index].Length; i++)
                    {
                        var item = _guides[index][i];
                        root.Add(_keywords[item]);
                    }
                }

                p = next;
            }

            return root;
        }

        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public string? FindFirstOrDefault(string text)
        {
            var spNext = new Span<int>(_next);
            var spCheck = new Span<int>(_check);
            var spKey = new Span<int>(_key);
            var spDict = new Span<int>(_dict);
            var spText = text.AsSpan();

            var p = 0;

            foreach (var c in spText)
            {
                var t = spDict[c];
                if (t == 0)
                {
                    p = 0;
                    continue;
                }

                var next = spNext[p] + t;
                bool find = spKey[next] == t;
                if (!find && p != 0)
                {
                    p = 0;
                    next = spNext[0] + t;
                    find = spKey[next] == t;
                }

                if (!find) continue;

                var index = spCheck[next];
                if (index > 0)
                {
                    var item = _guides[index][0];
                    return _keywords[item];
                }

                p = next;
            }

            return null;
        }

        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public bool ContainsAny(string text)
        {
            var spNext = new Span<int>(_next);
            var spCheck = new Span<int>(_check);
            var spKey = new Span<int>(_key);
            var spDict = new Span<int>(_dict);
            var spText = text.AsSpan();

            var p = 0;
            foreach (var c in spText)
            {
                var t = spDict[c];
                if (t == 0)
                {
                    p = 0;
                    continue;
                }

                var next = spNext[p] + t;
                if (spKey[next] == t)
                {
                    if (spCheck[next] > 0)
                    {
                        return true;
                    }

                    p = next;
                }
                else
                {
                    p = 0;
                    next = spNext[p] + t;
                    if (spKey[next] == t)
                    {
                        if (spCheck[next] > 0)
                        {
                            return true;
                        }

                        p = next;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <returns></returns>
        public string Replace(string text, char replaceChar = '*')
        {
            var spNext = new Span<int>(_next);
            var spCheck = new Span<int>(_check);
            var spKey = new Span<int>(_key);
            var spDict = new Span<int>(_dict);
            var spText = text.AsSpan();

            var arr = GC.AllocateUninitializedArray<char>(text.Length);
            text.AsSpan(0, text.Length).CopyTo(arr);
            var result = new Span<char>(arr);

            var p = 0;

            for (int i = 0; i < spText.Length; i++)
            {
                var t = (char)spDict[spText[i]];
                if (t == 0)
                {
                    p = 0;
                    continue;
                }

                var next = spNext[p] + t;
                bool find = spKey[next] == t;
                if (!find && p != 0)
                {
                    p = 0;
                    next = spNext[p] + t;
                    find = spKey[next] == t;
                }

                if (!find) continue;
                var index = spCheck[next];
                if (index > 0)
                {
                    var maxLength = _keywords[_guides[index][0]].Length;
                    var start = i + 1 - maxLength;
                    for (int j = start; j <= i; j++)
                    {
                        result[j] = replaceChar;
                    }
                }

                p = next;
            }

            return result.ToString();
        }

        #endregion
    }
}