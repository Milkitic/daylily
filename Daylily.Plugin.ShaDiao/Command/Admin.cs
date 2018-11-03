using Daylily.Bot;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daylily.Plugin.ShaDiao
{
    [Name("管理员菜单")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Beta)]
    [Help("咕4鸽2菜单。")]
    [Command("admin")]
    class Admin : CommandPlugin
    {
        private Session _session;
        private CommonMessage _cm;

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            throw new NotImplementedException();
            _cm = messageObj;
            try
            {
                using (_session = new Session(1000 * (60 * 2), _cm.Identity, _cm.UserId))
                {
                    try
                    {
                        const string mainMenu = "· 管理员菜单：\r\n" +
                                                " 1. 群员指令\r\n" +
                                                " 2. 投票指令\r\n";
                        SendMessage(new CommonMessageResponse(mainMenu, _cm));
                        CommonMessage cmMain = SessionCondition("1", "2");
                        switch (cmMain.Message)
                        {
                            case "1":
                                const string playerMenu = "· 群员指令：\r\n" +
                                                          " 1. 处死群员\r\n" +
                                                          " 2. 复活群员\r\n" +
                                                          " 3. 踢出群员\r\n" +
                                                          " 4. 封禁群员\r\n" +
                                                          " 5. 烧烤群员\r\n" +
                                                          " 6. 点群员灯\r\n" +
                                                          " 7. 群员更名\r\n\r\n" +
                                                          " 8. 把群员变成定时炸弹\r\n";
                                SendMessage(new CommonMessageResponse(playerMenu, _cm));
                                CommonMessage cmPlayer = SessionCondition("1", "2", "3", "4", "5", "6", "7", "8");
                                if (cmPlayer.Message == "1")
                                {
                                    return new CommonMessageResponse("删除成功。使用/m4m重新发布地图。", _cm);
                                }
                                else
                                    return new CommonMessageResponse("你已取消操作。", _cm);
                            case "2":
                                break;
                        }
                    }
                    catch (TimeoutException e)
                    {

                    }
                }
            }
            catch (NotSupportedException)
            {

            }

        }

        /// <summary>
        /// 单选会话
        /// </summary>
        private CommonMessage SessionCondition(params string[] conditions)
        {
            _session.Timeout = 30000;
            CommonMessage cmPub = _session.GetMessage();
            int retryCount = 0;
            while (!conditions.Contains(cmPub.Message) && retryCount < 3)
            {
                //SendMessage
                //(conditions.Length < 4
                //    ? new CommonMessageResponse($"请回复 \"{string.Join("\", \"", conditions)}\" 其中一个。", _cm)
                //    : new CommonMessageResponse($"请回复形如 \"{conditions[0]}\" 的选项。", _cm));
                //cmPub = _session.GetMessage();
                retryCount++;
            }

            return cmPub;
        }
    }
}
