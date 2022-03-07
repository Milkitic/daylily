namespace daylily;

public static class DefaultReply
{
    public const string PrivateOnly = "此功能仅限私聊。";
    public const string GroupDiscussOnly = "此功能仅限群聊。";

    public const string RootOnly = "此功能需要顶级权限（尝试/root [command]）";
    public const string AdminOnly = "此功能需要群内管理员权限（尝试/admin [command]）";

    public const string ParamError = "命令后参数不正确，请使用 /help [命令] 查看说明。";
    public const string ParamMissing = "请填写参数，请使用 /help [命令] 查看说明。";

    public static string FakeRoot
    {
        get
        {
            string[] exp = { "你没有顶级权限。" };
            return exp[Random.Shared.Next(0, exp.Length)];
        }
    }

    public const string FakeAdmin = "你非此群管理员，无法使用提权命令。";
}