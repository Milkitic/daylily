using Daylily.Bot.Backend;
using Daylily.Bot.Session;
using Daylily.Bot.Session.TreeStructure;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using System;
using System.Linq;
using Daylily.CoolQ.Plugin;
using Action = Daylily.Bot.Session.TreeStructure.Action;
using Session = Daylily.Bot.Session.Session;

namespace Daylily.Plugin.Fun
{
    [Name("管理员菜单")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Beta)]
    [Help("咕4鸽2菜单。")]
    [Command("admin")]
    class Admin : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("e5b99b9b-4165-4e4d-9ed2-7117b3c12787");

        private Session _session;
        private CoolQRouteMessage _routeMsg;

        const string mainNode = "Main";
        const string memberMenuNode = "memberMenu";
        
        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            _routeMsg = routeMsg;
            try
            {
                using (_session = new Session(1000 * (60 * 2), _routeMsg.Identity, _routeMsg.UserId))
                {
                    try
                    {
                        NavigatableNode memberMenu, voteMenu, inputQqIdScene, handleScene;
                        InitNode(out memberMenu, out voteMenu, out inputQqIdScene, out handleScene);
                        var program = new NavigatableTree(mainNode, obj =>
                        {
                            const string mainText = "· 管理员菜单：\r\n" +
                                                    " 1. 群员指令\r\n" +
                                                    " 2. 投票指令";
                            SendMessageAsync(routeMsg.ToSource(mainText));
                            CoolQRouteMessage cmMain = SessionCondition("1", "2");
                            switch (cmMain.RawMessage)
                            {
                                case "1":
                                    return new Action(memberMenuNode);
                                default:
                                    return new Action();
                            }
                        });

                        program.Root.AddChild(memberMenu);
                        program.Root.AddChild(voteMenu);
                        memberMenu.AddChild(inputQqIdScene);
                        inputQqIdScene.AddChild(handleScene);
                        program.Run();

                    }
                    catch (TimeoutException e)
                    {

                    }
                }
            }
            catch (NotSupportedException)
            {

            }

            return null;
        }

        private void InitNode(out NavigatableNode memberMenu, out NavigatableNode voteMenu,
            out NavigatableNode inputQqIdScene, out NavigatableNode handleScene)
        {
            memberMenu = new NavigatableNode(memberMenuNode, obj =>
            {
                const string memberText = "· 群员指令：\r\n" +
                                          " 1. 处死群员\r\n" +
                                          " 2. 复活群员\r\n" +
                                          " 3. 踢出群员\r\n" +
                                          " 4. 封禁群员\r\n" +
                                          " 5. 烧烤群员\r\n" +
                                          " 6. 点群员灯\r\n" +
                                          " 7. 群员更名\r\n" +
                                          " 8. 把群员变成定时炸弹\r\n" +
                                          " 9. 返回";
                SendMessageAsync(_routeMsg.ToSource(memberText));
                CoolQRouteMessage cmPlayer = SessionCondition("1", "2", "3", "4", "5", "6", "7", "8", "9");
                switch (cmPlayer.RawMessage)
                {
                    case "9":
                        return new Action(mainNode);
                    default:
                        return new Action();
                }
            });
            voteMenu = null;
            inputQqIdScene = null;
            handleScene = null;
        }

        /// <summary>
        /// 单选会话
        /// </summary>
        private CoolQRouteMessage SessionCondition(params string[] conditions)
        {
            _session.Timeout = 30000;
            CoolQRouteMessage cmPub = (CoolQRouteMessage)_session.GetMessage();
            int retryCount = 0;
            while (!conditions.Contains(cmPub.RawMessage) && retryCount < 3)
            {
                retryCount++;
                cmPub = (CoolQRouteMessage)_session.GetMessage();
            }

            return cmPub;
        }
    }
}
