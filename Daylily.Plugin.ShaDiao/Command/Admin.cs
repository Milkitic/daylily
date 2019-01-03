using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Bot.Sessions;
using Daylily.Bot.Sessions.TreeStructure;
using Action = Daylily.Bot.Sessions.TreeStructure.Action;

namespace Daylily.Plugin.ShaDiao
{
    [Name("管理员菜单")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Beta)]
    [Help("咕4鸽2菜单。")]
    [Command("admin")]
    public class Admin : CommandPlugin
    {
        private Session _session;
        private CommonMessage _cm;

        const string mainNode = "Main";
        const string memberMenuNode = "memberMenu";

        public override void OnInitialized(string[] args)
        {

        }

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {

            _cm = messageObj;
            try
            {
                using (_session = new Session(1000 * (60 * 2), _cm.CqIdentity, _cm.UserId))
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
                            SendMessage(new CommonMessageResponse(mainText, _cm));
                            CommonMessage cmMain = SessionCondition("1", "2");
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
                SendMessage(new CommonMessageResponse(memberText, _cm));
                CommonMessage cmPlayer = SessionCondition("1", "2", "3", "4", "5", "6", "7", "8", "9");
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
        private CommonMessage SessionCondition(params string[] conditions)
        {
            _session.Timeout = 30000;
            CommonMessage cmPub = _session.GetMessage();
            int retryCount = 0;
            while (!conditions.Contains(cmPub.RawMessage) && retryCount < 3)
            {
                retryCount++;
                cmPub = _session.GetMessage();
            }

            return cmPub;
        }
    }
}
