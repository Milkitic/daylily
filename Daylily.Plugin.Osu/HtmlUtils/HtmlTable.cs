using Daylily.Common.Text;
using System.Collections.Generic;

namespace Daylily.Plugin.Osu.HtmlUtils
{
    public class HtmlTable
    {
        private readonly string _htmlStr;
        public TableHead TableHead { get; }
        public TableBody TableBody { get; }
        public TableFoot TableFoot { get; }
        public int Width
        {
            get
            {
                int max = 0;
                if (TableHead?.Width > max)
                    max = TableHead.Width;
                if (TableBody?.Width > max)
                    max = TableBody.Width;
                if (TableFoot?.Width > max)
                    max = TableFoot.Width;
                return max;
            }
        }

        public int Height => (TableHead?.Height ?? 0) + (TableBody?.Height ?? 0) + (TableFoot?.Height ?? 0);

        public HtmlTable(string htmlStr)
        {
            _htmlStr = htmlStr.Replace("\r", "").Replace("\n", "");
            StringFinder sf = new StringFinder(_htmlStr);
            if (sf.FindNext("<thead") != -1)
            {
                TableHead = new TableHead();
                sf.FindNext("</thead>", false);
                string ori = sf.Cut();
                SetRows(TableHead, ori, "tr");
            }

            if (sf.FindNext("<tbody") != -1)
            {
                TableBody = new TableBody();
                sf.FindNext("</tbody>");
                string ori = sf.Cut();
                SetRows(TableBody, ori, "tr");
            }

            if (sf.FindNext("<tfoot") != -1)
            {
                TableFoot = new TableFoot();
                sf.FindNext("</tfoot>");
                string ori = sf.Cut();
                SetRows(TableFoot, ori, "tr");
            }
        }

        public string[,] GetArray()
        {
            string[,] sb = new string[Height, Width];
            int offset = 0;
            for (int i = 0; i < TableHead?.Height; i++, offset++)
            {
                for (int j = 0; j < TableHead.Rows[i].Length; j++)
                {
                    sb[offset, j] = TableHead.Rows[i][j];
                }
            }

            for (int i = 0; i < TableBody?.Height; i++, offset++)
            {
                for (int j = 0; j < TableBody.Rows[i].Length; j++)
                {
                    sb[offset, j] = TableBody.Rows[i][j];
                }
            }

            for (int i = 0; i < TableFoot?.Height; i++, offset++)
            {
                for (int j = 0; j < TableFoot.Rows[i].Length; j++)
                {
                    sb[offset, j] = TableFoot.Rows[i][j];
                }
            }
            return sb;
        }

        private void SetRows(TableElement tableElement, string ori, string key)
        {
            StringFinder sf = new StringFinder(ori);
            while (sf.FindNext($"<{key}") != -1)
            {
                sf.FindNext($"</{key}>", false);
                string oriRow = sf.Cut();

                var list = GetColList(oriRow, "th", "td");

                tableElement.Rows.AddRow(list);
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
        public int Length => _cols.Count;
        public string this[int index] => _cols[index];

        private readonly List<string> _cols = new List<string>();
        public Cols(IEnumerable<string> strs)
        {
            _cols.AddRange(strs);
        }
    }
    public class Rows
    {
        public int Length => _rows.Count;
        public Cols this[int index] => _rows[index];

        private readonly List<Cols> _rows = new List<Cols>();
        public void AddRow(IEnumerable<string> strs)
        {
            _rows.Add(new Cols(strs));
        }
    }

    public abstract class TableElement
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
    public class TableHead : TableElement { }

    public class TableBody : TableElement { }

    public class TableFoot : TableElement { }
}
