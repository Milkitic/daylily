using Daylily.Bot.Command;
using Daylily.Common.Logging;
using System;

namespace Daylily.Bot.Console
{
    internal class DaylilyConsole
    {
        public void WsCall(object metadata, string message)
        {
            Logger.Raw("> " + message);
            Response(message);
        }

        public void Response(string command)
        {
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
            var cmd = ca.Analyze(command);

            try
            {
                switch (cmd.CommandName)
                {
                    case "stop":
                        Logger.Raw("Application is shutting down... ");
                        Environment.Exit(0);
                        break;
                    case "console":
                        {
                            if (cmd.Args.ContainsKey("cut-count"))
                            {
                                if (cmd.Switches.Contains("set"))
                                {
                                    SocketLogger.CutCount = int.Parse(cmd.Args["cut-count"]);
                                    Logger.Raw(cmd.CommandName + ": set: oparation succeed");
                                }
                                else
                                {
                                    Logger.Raw(cmd.CommandName + ": parameter mismatch");
                                }
                            }
                            else if (cmd.Switches.Contains("get"))
                            {
                                if (cmd.Switches.Contains("cut-count"))
                                {
                                    Logger.Raw(SocketLogger.CutCount.ToString());
                                }
                            }
                            else
                                Logger.Raw(cmd.CommandName + ": parameter mismatch");
                            break;
                        }
                    default:
                        Logger.Raw(cmd.CommandName + ": command not found");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Raw(ex.Message);
            }
        }
    }
}
