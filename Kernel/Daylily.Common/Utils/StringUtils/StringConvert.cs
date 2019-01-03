using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Daylily.Common.Utils.StringUtils
{
    public static class StringConvert
    {
        public static string ToUnderLineStyle(this string source)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirstUpper = true;
            foreach (var c in source)
            {
                if (c >= 65 && c <= 90)
                {
                    if (!isFirstUpper)
                        sb.Append("_");
                    else
                        isFirstUpper = false;
                    sb.Append(c.ToString().ToLower());
                }
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static string ToJsonFormat(this string source)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(source);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj == null)
                return source;
            StringWriter textWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            serializer.Serialize(jsonWriter, obj);
            return textWriter.ToString();
        }
    }
}
