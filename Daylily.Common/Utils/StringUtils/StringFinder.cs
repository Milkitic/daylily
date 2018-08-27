using System;

namespace Daylily.Common.Utils.StringUtils
{
    public class StringFinder
    {
        public int StartIndex { get; private set; }
        public int EndIndex { get; private set; }
        public int Length => EndIndex - StartIndex;

        private readonly string _originStr;

        public StringFinder(string originStr)
        {
            _originStr = originStr;
        }

        public int FindNext(string str, bool ignoreLength = true)
        {
            StartIndex = EndIndex;
            EndIndex = ignoreLength
                ? _originStr.IndexOf(str, StartIndex, StringComparison.Ordinal)
                : _originStr.IndexOf(str, StartIndex, StringComparison.Ordinal) + str.Length;
            return EndIndex;
        }

        public void FindToLast()
        {
            StartIndex = EndIndex;
            EndIndex = _originStr.Length - 1;
        }
        public string Cut()
        {
            return _originStr.Substring(StartIndex, Length);
        }
        public void GetBound(out int startIndex, out int length)
        {
            startIndex = StartIndex;
            length = EndIndex - StartIndex;
        }

        public void Reset()
        {
            StartIndex = 0;
            EndIndex = 0;
        }

        public override string ToString()
        {
            return _originStr;
        }
    }
}
