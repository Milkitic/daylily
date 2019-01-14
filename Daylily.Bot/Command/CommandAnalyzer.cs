using System.Collections.Generic;

namespace Daylily.Bot.Command
{
    public class CommandAnalyzer
    {
        protected IParamDivider Divider;

        public CommandAnalyzer(IParamDivider divider)
        {
            this.Divider = divider;
        }

        public ICommand Analyze(string fullCmd)
        {
            Divider.TryDivide(fullCmd, out var cmd);
            return cmd;
        }

        public void Analyze(string fullCmd, IWritableCommand command)
        {
            Divider.TryDivide(fullCmd, out var cmd);
            command.CommandName = cmd.CommandName;
            command.Args = cmd.Args;
            command.FreeArgs = cmd.FreeArgs;
            command.Switches = cmd.Switches;
            command.SimpleArgs = cmd.SimpleArgs;
            command.ArgString = cmd.ArgString;
        }
    }

    public class CommandAnalyzer<T> : CommandAnalyzer where T : IParamDivider, new()
    {
        public CommandAnalyzer() : base(null)
        {
            Divider = new T();
        }
    }
}
