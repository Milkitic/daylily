using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Function.Command;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Models.MessageList;
using Daylily.Common.Utils;

namespace Daylily.Common.Function
{
    public class MessageHandler
    {
        private static GroupList GroupInfo { get; } = new GroupList();
        private static DiscussList DiscussInfo { get; } = new DiscussList();
        private static PrivateList PrivateInfo { get; } = new PrivateList();

        public static string CommandFlag = "!";

        //private readonly Random _rnd = new Random();
        //private const int MinTime = 200; // 回应的反应时间
        //private const int MaxTime = 300; // 回应的反应时间

        /// <summary>
        /// 群聊消息
        /// </summary>
        public MessageHandler(GroupMsg parsedObj)
        {
            long id = parsedObj.GroupId;

            GroupInfo.Add(id);
            if (GroupInfo[id].MsgQueue.Count < GroupInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                GroupInfo[id].MsgQueue.Enqueue(parsedObj);

            else if (!GroupInfo[id].LockMsg)
            {
                GroupInfo[id].LockMsg = true;
                AppConstruct.SendMessage(new CommonMessageResponse(parsedObj.Message, new CommonMessage(parsedObj)));
            }

            if (!GroupInfo[id].TryRun(() => HandleGroupMessage(parsedObj)))
            {
                Logger.Info("当前已有" + GroupInfo[id].MsgQueue.Count + "条消息在" + GroupInfo[id].Info.GroupName + "排队");
            }
        }

        /// <summary>
        /// 讨论组消息
        /// </summary>
        public MessageHandler(DiscussMsg parsedObj)
        {
            long id = parsedObj.DiscussId;

            DiscussInfo.Add(id);
            if (DiscussInfo[id].MsgQueue.Count < DiscussInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                DiscussInfo[id].MsgQueue.Enqueue(parsedObj);

            else if (!DiscussInfo[id].LockMsg)
            {
                DiscussInfo[id].LockMsg = true;
                AppConstruct.SendMessage(new CommonMessageResponse(parsedObj.Message, new CommonMessage(parsedObj)));
            }

            if (!DiscussInfo[id].TryRun(() => HandleDiscussMessage(parsedObj)))
            {
                Logger.Info("当前已有" + DiscussInfo[id].MsgQueue.Count + "条消息在" + id + "排队");
            }
        }

        /// <summary>
        /// 私聊消息
        /// </summary>
        public MessageHandler(PrivateMsg parsedObj)
        {
            long id = parsedObj.UserId;

            PrivateInfo.Add(id);
            if (PrivateInfo[id].MsgQueue.Count < PrivateInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                PrivateInfo[id].MsgQueue.Enqueue(parsedObj);

            else if (!PrivateInfo[id].LockMsg)
            {
                PrivateInfo[id].LockMsg = true;
                AppConstruct.SendMessage(new CommonMessageResponse("？？求您慢点说话好吗", new CommonMessage(parsedObj)));
            }

            if (!PrivateInfo[id].TryRun(() => HandlePrivateMessage(parsedObj)))
            {
                Logger.Info("当前已有" + PrivateInfo[id].MsgQueue.Count + "条消息在" + id + "排队");
            }
        }

        private void HandleGroupMessage(GroupMsg obj)
        {
            var parsedObj = obj;
            long groupId = parsedObj.GroupId;

            while (GroupInfo[groupId].MsgQueue.TryDequeue(out var currentInfo))
            {
                try
                {
                    CommonMessage commonMessage = new CommonMessage(currentInfo);
                    HandleMessage(commonMessage);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            GroupInfo[groupId].LockMsg = false;
        }

        private void HandleDiscussMessage(DiscussMsg obj)
        {
            var parsedObj = obj;

            long discussId = parsedObj.DiscussId;
            while (DiscussInfo[discussId].MsgQueue.TryDequeue(out var currentInfo))
            {
                try
                {
                    CommonMessage commonMessage = new CommonMessage(currentInfo);
                    HandleMessage(commonMessage);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            DiscussInfo[discussId].LockMsg = false;
        }

        private void HandlePrivateMessage(PrivateMsg obj)
        {
            var parsedObj = obj;

            long userId = parsedObj.UserId;
            while (PrivateInfo[userId].MsgQueue.TryDequeue(out var currentInfo))
            {
                try
                {
                    CommonMessage commonMessage = new CommonMessage(currentInfo);
                    HandleMessage(commonMessage);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            PrivateInfo[userId].LockMsg = false;
        }

        private void HandleMessage(CommonMessage commonMessage)
        {
            long groupId = Convert.ToInt64(commonMessage.GroupId);
            long userId = Convert.ToInt64(commonMessage.UserId);
            long discussId = Convert.ToInt64(commonMessage.DiscussId);
            var type = commonMessage.MessageType;
            string message = commonMessage.Message;

            switch (commonMessage.MessageType)
            {
                case MessageType.Private:
                    Logger.Message($"{userId}:\r\n  {CqCode.Decode(message)}");
                    break;
                case MessageType.Discuss:
                    Logger.Message($"({DiscussInfo[discussId].Name}) {userId}:\r\n  {CqCode.Decode(message)}");
                    break;
                case MessageType.Group:
                    var userInfo = CqApi.GetGroupMemberInfo(groupId.ToString(), userId.ToString()); // 有点费时间
                    Logger.Message(string.Format("({0}) {1}:\r\n  {2}", GroupInfo[groupId].Info.GroupName,
                        string.IsNullOrEmpty(userInfo.Data.Card) ? "(n)" + userInfo.Data.Nickname : userInfo.Data.Card,
                        CqCode.Decode(message)));
                    break;
            }

            if (commonMessage.Message.Substring(0, 1) == CommandFlag)
            {
                if (commonMessage.Message.IndexOf(CommandFlag + "root ", StringComparison.Ordinal) == 0)
                {
                    if (commonMessage.UserId != "2241521134")
                    {
                        AppConstruct.SendMessage(new CommonMessageResponse(LoliReply.FakeRoot, commonMessage));
                    }
                    else
                    {
                        commonMessage.FullCommand =
                            commonMessage.Message.Substring(6, commonMessage.Message.Length - 6);
                        commonMessage.PermissionLevel = PermissionLevel.Root;
                        HandleMessageCmd(commonMessage);
                    }

                }
                else if (message.IndexOf(CommandFlag + "sudo ", StringComparison.Ordinal) == 0 &&
                         type == MessageType.Group)
                {
                    if (GroupInfo[groupId].Info.Admins.Count(q => q.UserId == userId) == 0)
                    {
                        AppConstruct.SendMessage(new CommonMessageResponse(LoliReply.FakeAdmin, commonMessage));
                    }
                    else
                    {
                        commonMessage.FullCommand = message.Substring(6, message.Length - 6);
                        commonMessage.PermissionLevel = PermissionLevel.Admin;
                        HandleMessageCmd(commonMessage);
                    }
                }
                else
                {
                    commonMessage.FullCommand = message.Substring(1, message.Length - 1);
                    HandleMessageCmd(commonMessage);
                }
            }

            HandleMesasgeApp(commonMessage);
        }

        private static void HandleMesasgeApp(CommonMessage commonMessage)
        {
            foreach (var item in PluginManager.ApplicationList)
            {
                CommonMessageResponse replyObj = item.OnExecute(commonMessage);

                if (replyObj == null) continue;
                AppConstruct.SendMessage(replyObj);
            }
        }

        private static void HandleMessageCmd(CommonMessage commonMessage)
        {
            //Thread.Sleep(_rnd.Next(MinTime, MaxTime));

            string fullCmd = commonMessage.FullCommand;
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());

            ca.Analyze(fullCmd);
            commonMessage.Command = ca.CommandName;
            commonMessage.Parameter = ca.Parameter;
            commonMessage.Parameters = ca.Parameters;
            commonMessage.Switches = ca.Switches;
            commonMessage.SimpleParams = ca.SimpleParams;

            CommonMessageResponse replyObj = null;
            AppConstruct plugin = null;
            if (!PluginManager.CommandMap.ContainsKey(commonMessage.Command)) return;
            try
            {
                plugin = PluginManager.CommandMap[commonMessage.Command];
                replyObj = plugin.OnExecute(commonMessage);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, fullCmd, plugin?.ToString() ?? "Unknown plugin");
            }
            if (replyObj == null) return;
            AppConstruct.SendMessage(replyObj);
        }
    }
}
