using System.Collections.Generic;
using Daylily.Common.Utils.StringUtils;

namespace Daylily.Common.Utils.HtmlUtils
{
    public class HtmlTable
    {
        private readonly string _htmlStr;
        public THead Head { get; }
        public TBody Body { get; }
        public TFoot Foot { get; }
        public int Width
        {
            get
            {
                int max = 0;
                if (Head?.Width > max)
                    max = Head.Width;
                if (Body?.Width > max)
                    max = Body.Width;
                if (Foot?.Width > max)
                    max = Foot.Width;
                return max;
            }
        }

        public int Height => (Head == null ? 0 : Head.Height) + (Body == null ? 0 : Body.Height) +
                             (Foot == null ? 0 : Foot.Height);

        public HtmlTable(string htmlStr)
        {
            _htmlStr = htmlStr.Replace("\r", "").Replace("\n", "");
            StringFinder sfHBF = new StringFinder(_htmlStr);
            if (sfHBF.FindNext("<thead") != -1)
            {
                Head = new THead();
                sfHBF.FindNext("</thead>", false);
                string ori = sfHBF.Cut();
                SetRows(Head, ori, "tr");
            }

            if (sfHBF.FindNext("<tbody") != -1)
            {
                Body = new TBody();
                sfHBF.FindNext("</tbody>");
                string ori = sfHBF.Cut();
                SetRows(Body, ori, "tr");
            }

            if (sfHBF.FindNext("<tfoot") != -1)
            {
                Foot = new TFoot();
                sfHBF.FindNext("</tfoot>");
                string ori = sfHBF.Cut();
                SetRows(Foot, ori, "tr");
            }
        }

        public string[,] GetArray()
        {
            string[,] sb = new string[Height, Width];
            int offset = 0;
            for (int i = 0; i < Head?.Height; i++, offset++)
            {
                for (int j = 0; j < Head.Rows[i].Length; j++)
                {
                    sb[offset, j] = Head.Rows[i][j];
                }
            }

            for (int i = 0; i < Body?.Height; i++, offset++)
            {
                for (int j = 0; j < Body.Rows[i].Length; j++)
                {
                    sb[offset, j] = Body.Rows[i][j];
                }
            }

            for (int i = 0; i < Foot?.Height; i++, offset++)
            {
                for (int j = 0; j < Foot.Rows[i].Length; j++)
                {
                    sb[offset, j] = Foot.Rows[i][j];
                }
            }
            return sb;
        }

        private void SetRows(TElement element, string ori, string key)
        {
            StringFinder sfTR = new StringFinder(ori);
            while (sfTR.FindNext($"<{key}") != -1)
            {
                sfTR.FindNext($"</{key}>", false);
                string oriRow = sfTR.Cut();

                var list = GetColList(oriRow, "th", "td");

                element.Rows.AddRow(list);
            }
        }

        private static IEnumerable<string> GetColList(string ori, params string[] mark)
        {
            List<string> param = new List<string>();

            foreach (var item in mark)
            {
                StringFinder sf = new StringFinder(ori);
                while (sf.FindNext($"<{item}") != -1)
                {
                    sf.FindNext($"</{item}>", false);
                    string ok = sf.Cut();
                    string trimed = TrimHtml(ok, item);
                    param.Add(trimed);
                }
            }

            return param;
        }

        private static string TrimHtml(string ok, string mark)
        {
            StringFinder sf = new StringFinder(ok);
            sf.FindNext($"<{mark}");
            sf.FindNext(">", false);
            sf.FindNext($"</{mark}>");
            return sf.Cut();
        }
    }

    public class Cols
    {
        private List<string> _cols { get; set; } = new List<string>();
        public int Length => _cols.Count;
        public string this[int index] => _cols[index];

        public Cols(IEnumerable<string> strs)
        {
            _cols.AddRange(strs);
        }
    }
    public class Rows
    {
        private List<Cols> _rows { get; set; } = new List<Cols>();
        public int Length => _rows.Count;
        public Cols this[int index] => _rows[index];

        public void AddRow(IEnumerable<string> strs)
        {
            _rows.Add(new Cols(strs));
        }
    }

    public abstract class TElement
    {
        public Rows Rows { get; set; } = new Rows();

        public int Width
        {
            get
            {
                int max = 0;
                for (int i = 0; i < Rows.Length; i++)
                {
                    if (Rows[i].Length > max)
                        max = Rows[i].Length;
                }
                return max;
            }
        }

        public int Height => Rows.Length;
    }
    public class THead : TElement { }

    public class TBody : TElement { }

    public class TFoot : TElement { }
}
