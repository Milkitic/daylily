namespace Daylily.Bot.Sessions.TreeStructure
{
    public struct ActionParam
    {
        public ActionParam(string targetName) : this()
        {
            TargetName = targetName;
        }

        public ActionParam(string targetName, object targetParam) : this()
        {
            TargetName = targetName;
            TargetParam = targetParam;
        }

        public string TargetName { get; set; }
        public object TargetParam { get; set; }
    }
}