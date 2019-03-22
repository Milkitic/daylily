using Daylily.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daylily.Bot.Command
{
    public class ParamDividerV2 : IParamDivider
    {
        private static readonly char[] Quote = { '\'', '\"' };
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
            var simpleArgs = argString.Split(' ').ToList();

            if (argString == "")
            {
                command = new Command(commandName, args, freeArgs, switches, fullCmd, argString, simpleArgs);
                return false;
            }

            var splitParam = new List<string>();
            try
            {
                splitParam.AddRange(argString.Split(' '));
                foreach (var item in splitParam)
                {
                    if (Quote.Any(q => ContainsChar(q, item)))
                    {
                        throw new ArgumentException();
                    }
                }

                bool combined = true;
                foreach (var item in Quote)
                {
                    for (int i = 0; i < splitParam.Count - 1; i++)
                    {
                        string cur = splitParam[i], next = splitParam[i + 1];

                        if (cur.StartsWith(item) && !cur.EndsWith(item))
                        {
                            combined = false;
                            splitParam[i] = cur + " " + next;
                            splitParam.Remove(next);
                            if (splitParam[i].EndsWith(item))
                                combined = true;
                            i--;
                        }
                    }
                    if (!combined) throw new ArgumentException("Expect '" + item + "'.");
                }

                string tmpKey = null;
                bool isLastKeyOrValue = false;

                splitParam.Add("-");
                foreach (var item in splitParam)
                {
                    string tmpValue = null;
                    if (item.StartsWith('-'))
                    {
                        if (tmpKey != null)
                        {
                            switches.Add(tmpKey);
                        }

                        tmpKey = item.Remove(0, 1);
                        isLastKeyOrValue = true;
                    }
                    else
                    {
                        foreach (var q in Quote)
                        {
                            tmpValue = tmpValue == null ? item.Trim(q) : tmpValue.Trim(q);
                        }
                        if (!isLastKeyOrValue)
                        {
                            freeArgs.Add(tmpValue);
                            //throw new ArgumentException("Expect key.");
                        }
                        else
                        {
                            args.Add(tmpKey, tmpValue);
                            tmpKey = null;
                            tmpValue = null;
                            isLastKeyOrValue = false;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                command = new Command(commandName, args, freeArgs, switches, fullCmd, argString, simpleArgs);
                return false;
            }

            command = new Command(commandName, args, freeArgs, switches, fullCmd, argString, simpleArgs);
            return true;
        }

        private bool ContainsChar(char ch, string str)
        {
            char[] cs = str.ToCharArray();
            for (int i = 1; i < cs.Length - 1; i++)
            {
                if (cs[i] == ch)
                    return true;
            }

            return false;
        }
    }
}

