using System;
using System.Collections.Generic;

namespace Daylily.Bot.Command
{
    public class ParamDividerV1 : IParamDivider
    {
        public bool TryDivide(string fullCmd, out ICommand command)
        {
            var commandName = fullCmd.Split(' ')[0].Trim();
            var argString = fullCmd.IndexOf(" ", StringComparison.Ordinal) == -1
                    ? ""
                    : fullCmd.Substring(fullCmd.IndexOf(" ", StringComparison.Ordinal) + 1,
                        fullCmd.Length - commandName.Length - 1).Trim();
            var args = new Dictionary<string, string>();
            var freeArgs = new List<string>();
            var switches = new List<string>();
            var simpleArgs = new List<string>();

            char quoteFlag = '\0';
            bool isKeyOrValue = true;
            int startP = -1, endP = -1;
            string tmpKey = "", tmpValue = "";
            string param = argString + " ";
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
                                    switches.Add(tmpKey);
                                    isKeyOrValue = false;
                                }
                                else
                                {
                                    tmpValue = param.Substring(startP, endP - startP).Trim().Trim('\"');
                                    args.Add(tmpKey, tmpValue);
                                    switches.Remove(tmpKey);
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
                command = new Command(commandName, args, freeArgs, switches, fullCmd, argString, simpleArgs);
                return false;
            }

            command = new Command(commandName, args, freeArgs, switches, fullCmd, argString, simpleArgs);
            return true;
        }
    }
}
