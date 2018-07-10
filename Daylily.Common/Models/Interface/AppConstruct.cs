using System;
using System.IO;
using Daylily.Common.Assist;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models.CQResponse.Api;
using Daylily.Common.Models.CQResponse.Api.Abstract;
using Daylily.Common.Models.Enum;
using Daylily.Common.Utils;

namespace Daylily.Common.Models.Interface
{
    public abstract class AppConstruct
    {
        protected PermissionLevel CurrentLevel { get; set; }

        protected static readonly Random Rnd = new Random();

        /// <summary>
        /// 以逗号分隔 如"sleep,slip"
        /// </summary>
        public abstract string Name { get; }
        public abstract string Author { get; }
        public abstract PluginVersion Version { get; }
        public abstract string VersionNumber { get; }
        public abstract string Description { get; }
        public abstract string Command { get; }
        public abstract AppType AppType { get; }
        public abstract void OnLoad(string[] args);
        public abstract CommonMessageResponse OnExecute(in CommonMessage messageObj);

        public static void SendMessage(CommonMessageResponse response)
        {
            switch (response.MessageType)
            {
                case MessageType.Group:
                    SendGroupMsgResp groupMsgResp = CqApi.SendGroupMessageAsync(response.GroupId,
                        (response.EnableAt ? new At(response.UserId) + " " : "") + response.Message);
                    Logger.Info($"我: {CqCode.Decode(response.Message)} {{status: {groupMsgResp.Status}}})");
                    break;
                case MessageType.Discuss:
                    SendDiscussMsgResp discussMsgResp = CqApi.SendDiscussMessageAsync(response.DiscussId,
                        (response.EnableAt ? new At(response.UserId) + " " : "") + response.Message);
                    Logger.Info($"我: {CqCode.Decode(response.Message)} {{status: {discussMsgResp.Status}}})");
                    break;
                case MessageType.Private:
                    SendPrivateMsgResp privateMsgResp =
                        CqApi.SendPrivateMessageAsync(response.UserId, response.Message);
                    Logger.Info($"我: {CqCode.Decode(response.Message)} {{status: {privateMsgResp.Status}}})");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void SendMessage(CommonMessageResponse response, string groupId, string discussId,
            MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Group:
                    SendGroupMsgResp groupMsgResp = CqApi.SendGroupMessageAsync(groupId,
                        (response.EnableAt ? new At(response.UserId) + " " : "") + response.Message);
                    Logger.Info($"我: {CqCode.Decode(response.Message)} {{status: {groupMsgResp.Status}}})");
                    break;
                case MessageType.Discuss:
                    SendDiscussMsgResp discussMsgResp = CqApi.SendDiscussMessageAsync(discussId,
                        (response.EnableAt ? new At(response.UserId) + " " : "") + response.Message);
                    Logger.Info($"我: {CqCode.Decode(response.Message)} {{status: {discussMsgResp.Status}}})");
                    break;
                case MessageType.Private:
                    SendPrivateMsgResp privateMsgResp =
                        CqApi.SendPrivateMessageAsync(response.UserId, response.Message);
                    Logger.Info($"我: {CqCode.Decode(response.Message)} {{status: {privateMsgResp.Status}}})");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private readonly string _pluginDir = Path.Combine(Domain.CurrentDirectory, "plugins");

        protected void SaveSettings<T>(T cls, string fileName = null)
        {
            Type thisT = GetType();
            Type clsT = cls.GetType();

            string setsDir = Path.Combine(_pluginDir, thisT.Name);
            string saveName = Path.Combine(setsDir, (fileName ?? clsT.Name) + ".json");

            if (!Directory.Exists(setsDir))
                Directory.CreateDirectory(setsDir);

            File.WriteAllText(saveName, Newtonsoft.Json.JsonConvert.SerializeObject(cls));
        }

        protected T LoadSettings<T>(string fileName = null)
        {
            try
            {
                Type thisT = GetType();
                Type clsT = typeof(T);

                string setsDir = Path.Combine(_pluginDir, thisT.Name);
                string saveName = Path.Combine(setsDir, (fileName ?? clsT.Name) + ".json");

                if (!Directory.Exists(setsDir))
                    Directory.CreateDirectory(setsDir);

                string json = File.ReadAllText(saveName);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return default;
            }
        }
    }
}
