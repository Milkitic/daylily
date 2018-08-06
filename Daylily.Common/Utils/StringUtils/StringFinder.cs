using System;

namespace Daylily.Common.Utils.StringUtils
{
    public class StringFinder
    {
        private int _preIndex, _currentIndex;
        private readonly string _originStr;

        public StringFinder(string originStr)
        {
            _originStr = originStr;
        }

        public int FindNext(string str, bool ignoreLength = true)
        {
            _preIndex = _currentIndex;
            _currentIndex = ignoreLength
                ? _originStr.IndexOf(str, _preIndex, StringComparison.Ordinal)
                : _originStr.IndexOf(str, _preIndex, StringComparison.Ordinal) + str.Length;
            return _currentIndex;
        }

        public string Cut()
        {
            int length = _currentIndex - _preIndex;
            return _originStr.Substring(_preIndex, length);
        }

        public void Reset()
        {
            _preIndex = 0;
            _currentIndex = 0;
        }

        public override string ToString()
        {
            return _originStr;
        }
    }
}
