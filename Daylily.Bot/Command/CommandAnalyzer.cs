using System.Collections.Generic;

namespace Daylily.Bot.Command
{
    public class CommandAnalyzer
    {
        private readonly IParamDivider _divider;

        public CommandAnalyzer(IParamDivider divider)
        {
            this._divider = divider;
        }

        public ICommand Analyze(string fullCmd)
        {
            _divider.TryDivide(fullCmd, out var cmd);
            return cmd;
        }

        public void Analyze(string fullCmd, IWritableCommand command)
        {
            _divider.TryDivide(fullCmd, out var cmd);
            command.CommandName = cmd.CommandName;
            command.Args = cmd.Args;
            command.FreeArgs = cmd.FreeArgs;
            command.Switches = cmd.Switches;
            command.SimpleArgs = cmd.SimpleArgs;
            command.ArgString = cmd.ArgString;
        }
    }
}
