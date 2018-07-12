using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Function.Command;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Common.Console
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
            ca.Analyze(command);

            try
            {
                switch (ca.CommandName)
                {
                    case "stop":
                        Logger.Raw("Application is shutting down... ");
                        Environment.Exit(0);
                        break;
                    case "console":
                        {
                            if (ca.Parameters.ContainsKey("cut-count"))
                            {
                                if (ca.Switches.ContainsKey("set"))
                                {
                                    SocketLogger.CutCount = int.Parse(ca.Parameters["cut-count"]);
                                    Logger.Raw(ca.CommandName + ": set: oparation succeed");
                                }
                                else
                                {
                                    Logger.Raw(ca.CommandName + ": parameter mismatch");
                                }
                            }
                            else if (ca.Switches.ContainsKey("get"))
                            {
                                if (ca.Switches.ContainsKey("cut-count"))
                                {
                                    Logger.Raw(SocketLogger.CutCount.ToString());
                                }
                            }
                            else
                                Logger.Raw(ca.CommandName + ": parameter mismatch");
                            break;
                        }
                    default:
                        Logger.Raw(ca.CommandName + ": command not found");
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
