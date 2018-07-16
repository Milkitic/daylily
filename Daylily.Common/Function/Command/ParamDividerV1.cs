using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Common.Function.Command
{
    public class ParamDividerV1 : IParamDivider
    {
        public string CommandName { get; private set; }
        public string ArgString { get; private set; }

        public Dictionary<string, string> Args { get; } =
            new Dictionary<string, string>();

        public List<string> FreeArgs { get; } = new List<string>();
        public Dictionary<string, string> Switches { get; } =
            new Dictionary<string, string>();

        public List<string> SimpleArgs { get; set; } = new List<string>();

        public bool TryDivide(string fullCmd)
        {
            CommandName = fullCmd.Split(' ')[0].Trim();
            ArgString = fullCmd.IndexOf(" ", StringComparison.Ordinal) == -1
                ? ""
                : fullCmd.Substring(fullCmd.IndexOf(" ", StringComparison.Ordinal) + 1,
                    fullCmd.Length - CommandName.Length - 1).Trim();

            char quoteFlag = '\0';
            bool isKeyOrValue = true;
            int startP = -1, endP = -1;
            string tmpKey = "", tmpValue = "";
            string param = ArgString + " ";
            try
            {
                for (var i = 0; i < param.Length; i++)
                {
                    var item = param[i];
                    if (quoteFlag == '\0')
                    {
                        switch (item)
                        {
                            case '\'':
                            case '\"':
                                quoteFlag = item;
                                break;
                            case '-':
                                if (startP != -1 && isKeyOrValue)
                                {
                                    throw new ArgumentException();
                                    //Parameters.GetOrAdd(tmpKey, value: null);
                                    //Switches.GetOrAdd(tmpKey, tmpKey);
                                }
                                startP = i;
                                isKeyOrValue = true;
                                break;
                            case ' ':
                                endP = i;
                                if (isKeyOrValue)
                                {
                                    tmpKey = param.Substring(startP, endP - startP).Trim().TrimStart('-');
                                    Switches.Add(tmpKey, tmpKey);
                                    isKeyOrValue = false;
                                }
                                else
                                {
                                    tmpValue = param.Substring(startP, endP - startP).Trim().Trim('\"');
                                    Args.Add(tmpKey, tmpValue);
                                    Switches.Remove(tmpKey, out _);
                                }
                                startP = i;
                                break;
                        }
                    }
                    else
                    {
                        if (item == quoteFlag)
                        {
                            quoteFlag = '\0';
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
