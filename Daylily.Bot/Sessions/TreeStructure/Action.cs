namespace Daylily.Bot.Sessions.TreeStructure
{
    public struct Action
    {
        public Action(string targetName) : this()
        {
            TargetName = targetName;
        }

        public Action(string targetName, object targetParam) : this()
        {
            TargetName = targetName;
            TargetParam = targetParam;
        }

        public string TargetName { get; set; }
        public object TargetParam { get; set; }
    }
}