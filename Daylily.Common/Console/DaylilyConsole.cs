using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Function.Command;
using Daylily.Common.Utils;

namespace Daylily.Common.Console
{
    internal static class DaylilyConsole
    {
        public static void Response(string command)
        {
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
            ca.Analyze(command);
            switch (ca.CommandName)
            {
                case "stop":
                    Logger.Raw("Application is shutting down... ");
                    Environment.Exit(0);
                    break;
                default:
                    Logger.Raw(ca.CommandName + ": command not found");
                    break;
            }
        }
    }
}
