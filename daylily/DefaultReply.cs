namespace daylily;

public static class DefaultReply
{
    private static readonly string[] FakeRootList = { "你没有顶级权限。" };

    public const string PrivateOnly = "此功能仅限私聊。";
    public const string GroupDiscussOnly = "此功能仅限群聊。";

    public const string RootOnly = "此功能需要顶级权限（尝试/root [command]）";
    public const string AdminOnly = "此功能需要群内管理员权限（尝试/admin [command]）";

    public const string ParamError = "命令后参数不正确，请使用 /help [命令] 查看说明。";
    public const string ParamMissing = "请填写参数，请使用 /help [命令] 查看说明。";

    public static string FakeRoot => FakeRootList[Random.Shared.Next(0, FakeRootList.Length)];

    public const string FakeAdmin = "你非此群管理员，无法使用提权命令。";
}