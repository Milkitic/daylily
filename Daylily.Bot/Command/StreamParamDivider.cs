using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daylily.Bot.Command
{
    public class StreamParamDivider : IParamDivider
    {
        private static readonly char[] Keywords = { '/', '-' };
        private static readonly char[] Quotes = { '\"', '\'' };

        public bool TryDivide(string fullCmd, out ICommand command)
        {
            bool AddToSwitch(ICollection<string> list, ref string s)
            {
                list.Add(s);
                s = null;
                return false;
            }

            var reader = new StringReader(fullCmd);
            var builder = new StringBuilder();
            int r = reader.Read();

            bool isReadingArgOrSwitch = false;

            var args = new Dictionary<string, string>();
            var freeArgs = new List<string>();
            var switches = new List<string>();

            string commandName = null;
            string placeHolder = null;

            while (r != -1)
            {
                char c = (char)r;
                string value;

                if (Keywords.Contains(c)) // if the char is keyword
                {
                    if (placeHolder != null) // placeHolder is switch
                    {
                        isReadingArgOrSwitch = AddToSwitch(switches, ref placeHolder);
                    }

                    switch (c)
                    {
                        case '-': // switch or arg
                            isReadingArgOrSwitch = true;
                            break;
                    }

                    r = reader.Read();
                    continue;
                }
                else if (c == ' ') // skip the rest spaces
                {
                    r = reader.Read();
                    continue;
                }
                else
                {
                    builder.Clear();
                    if (Quotes.Contains(c))
                    {
                        value = ReadUntilQuote(reader, builder, c);
                    }
                    else
                    {
                        builder.Append(c);
                        value = ReadUntilSpaceOrEnd(reader, builder);
                    }
                    if (commandName == null)
                        commandName = value;
                    else
                    {
                        if (isReadingArgOrSwitch && placeHolder != null) // placeHolder is arg
                        {
                            args.Add(placeHolder, value);
                            placeHolder = null;
                            isReadingArgOrSwitch = false;
                        }
                        else if (!isReadingArgOrSwitch) // is freeArg
                        {
                            freeArgs.Add(value);
                        }
                    }
                }

                if (isReadingArgOrSwitch)
                {
                    placeHolder = value;
                }
                r = reader.Read();
            }

            if (placeHolder != null) // placeHolder is switch
            {
                AddToSwitch(switches, ref placeHolder);
            }

            command = new Command(commandName, args, freeArgs, switches, fullCmd,
                commandName == null ? null : fullCmd.Remove(0, commandName.Length), null);
            return true;
        }


        private static string ReadUntilCharOrEnd(TextReader reader, StringBuilder builder, char ch)
        {
            return ReadUntilChars(reader, builder, new[] { ch, unchecked((char)-1) });
        }

        private static string ReadUntilChar(TextReader reader, StringBuilder builder, char ch)
        {
            return ReadUntilChars(reader, builder, new[] { ch });
        }

        private static string ReadUntilChars(TextReader reader, StringBuilder builder, char[] ch)
        {
            int r = reader.Read();
            while (r != -1)
            {
                var c = (char)r;
                if (ch.Contains(c))
                {
                    break;
                }

                builder.Append(c);
                r = reader.Read();
            }

            if (r == -1 && !ch.Contains(unchecked((char)-1)))
                throw new ArgumentException();
            return builder.ToString();
        }

        private static string ReadUntilSpaceOrEnd(TextReader reader, StringBuilder builder)
        {
            return ReadUntilCharOrEnd(reader, builder, ' ');
        }

        private static string ReadUntilQuote(TextReader reader, StringBuilder builder, char quote)
        {
            return ReadUntilChar(reader, builder, quote);
        }
    }
}
