using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bleatingsheep.Osu.ApiV2b;
using Bleatingsheep.Osu.ApiV2b.Models;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Daylily.Osu.Interface;

namespace Daylily.Plugin.Osu.Command.Subscribes
{
    [Name("Mapper订阅")]
    [Author("yf_extension")]
    [Version(0, 1, 1, PluginVersion.Alpha)]
    [Help("订阅某个mapper的qua、rank、love、传图提醒。", "限制为群内推送8个名额，个人推送3个名额。")]
    [Command("sub")]
    public class Subscribe : CommandPlugin
    {
        [Arg("u")]
        [Help("需要取消订阅的mapper用户名。")]
        public string UnsubscribeMapper { get; set; }

        [Arg("l", IsSwitch = true)]
        [Help("若启用，查询已经订阅的mapper。")]
        public bool List { get; set; }

        [FreeArg]
        [Help("需要订阅的mapper用户名。")]
        public string SubscribeMapper { get; set; }

        private static ConcurrentDictionary<string, List<UserInfo>> _userDic;
        private static List<SlimBeatmapsets> _todaySets;
#if DEBUG
        private static readonly TimeSpan RangeTime = new TimeSpan(365, 0, 0, 0);
#else
        private static readonly TimeSpan RangeTime = new TimeSpan(24, 0, 0);
#endif
        private const int PrivateMax = 3;
        private const int GroupMax = 8;

        public override void Initialize(string[] args)
        {
            _userDic = LoadSettings<ConcurrentDictionary<string, List<UserInfo>>>("userDictionary") ??
                       new ConcurrentDictionary<string, List<UserInfo>>();
            _todaySets = LoadSettings<List<SlimBeatmapsets>>("today") ?? new List<SlimBeatmapsets>();

            Task.Run(() =>
            {
                // 扫描的频率，但未包括等待另一线程的时间，也就是>=这个间隔。
#if DEBUG
                const int sleepTime = 5000;
#else
                const int sleepTime = 1000 * 60 * 60; 
#endif

                while (true)
                {
                    // 由于其中会有很多阻塞 使用异步
                    Task asyncdTask = new Task(() =>
                    {
                        RemoveOverdueMapsets();

                        // 以mapper为单位的设计
                        foreach (var pair in _userDic)
                        {
                            try
                            {
                                string mapper = pair.Key;
                                List<UserInfo> userList = pair.Value;
                                if (pair.Value.Count == 0)
                                {
                                    RemoveUnusefulMapper(mapper);
                                    continue;
                                }

                                Beatmapsets[] mapsets = GetBeatmapsets(mapper);

                                if (mapsets.Length > 0)
                                {
                                    PushNews(userList, mapsets);
                                    foreach (var item in mapsets)
                                    {
                                        _todaySets.Add(new SlimBeatmapsets
                                        {
                                            Creator = item.Creator,
                                            Id = item.Id,
                                            RankedDate = item.RankedDate,
                                            Status = item.Status,
                                            SubmittedDate = item.SubmittedDate
                                        });
                                    }

                                    SaveTodaySettings();
                                }

                                Logger.Debug($"{pair.Key}: Find {mapsets.Length} results.");
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex);
                            }
                        }
                    });

                    asyncdTask.Start();
                    Thread.Sleep(sleepTime);
                    Task.WaitAll(asyncdTask); // 等待执行完毕
                }
            });
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            string subId;
            switch (messageObj.MessageType)
            {
                case MessageType.Private:
                    subId = messageObj.UserId;
                    break;
                case MessageType.Discuss:
                    subId = messageObj.DiscussId;
                    break;
                case MessageType.Group:
                default:
                    subId = messageObj.GroupId;
                    break;
            }

            if (List)
            {
                List<string> subedId = GetSubscribed(messageObj.MessageType, subId).ToList();
                subedId.Sort();
                int subedCount = subedId.Count;
                string subject = messageObj.MessageType == MessageType.Private ? "你" : "本群";
                return subedCount == 0
                    ? new CommonMessageResponse($"{subject}没有订阅任何mapper。", messageObj)
                    : new CommonMessageResponse(
                        string.Format("以下是{0}的订阅名单，共计{1}个：\r\n{2}", subject, subedCount,
                            string.Join("\r\n", OldSiteApi.GetUsernameByUid(subedId))),
                        messageObj);
            }

            if (SubscribeMapper != null)
            {
                if (messageObj.MessageType == MessageType.Group && messageObj.PermissionLevel == PermissionLevel.Public)
                    return new CommonMessageResponse(LoliReply.AdminOnly + "个人推送请私聊.", messageObj);

                List<string> subedId = GetSubscribed(messageObj.MessageType, subId).ToList();
                subedId.Sort();
                int subedCount = subedId.Count;
                if (messageObj.MessageType == MessageType.Private && subedCount >= PrivateMax)
                {
                    return new CommonMessageResponse(
                        string.Format("你已经订阅了{0}个mapper啦，人家已经装不下的说：{1}", subedCount,
                            string.Join(", ", OldSiteApi.GetUsernameByUid(subedId))), messageObj);
                }

                if (messageObj.MessageType != MessageType.Private && subedCount >= GroupMax)
                {
                    return new CommonMessageResponse(
                        string.Format("这个群已经订阅了{0}个mapper啦，人家已经装不下的说：{1}", subedCount,
                            string.Join(", ", OldSiteApi.GetUsernameByUid(subedId))), messageObj);
                }

                int count = OldSiteApi.GetUser(SubscribeMapper, out var userObj);
                if (count == 0)
                    return new CommonMessageResponse("找不到指定mapper..", messageObj);

                if (count > 1)
                    return new CommonMessageResponse($"找到{count}个mapper..", messageObj);

                string mapperId = userObj.user_id;
                string mapperName = userObj.username;

                if (!_userDic.ContainsKey(mapperId))
                    _userDic.TryAdd(mapperId, new List<UserInfo>());
                if (_userDic[mapperId].Contains(new UserInfo(subId, messageObj.MessageType)))
                {
                    string subject = messageObj.MessageType == MessageType.Private ? "你" : "本群";
                    return new CommonMessageResponse($"{subject}已经订阅过{mapperName}啦..", messageObj);
                }

                _userDic[mapperId].Add(new UserInfo(subId, messageObj.MessageType));
                SaveSettings(_userDic, "userDictionary");
                string sub = messageObj.MessageType == MessageType.Private ? "私聊提醒你" : "在本群提醒";
                return new CommonMessageResponse($"{mapperName}订阅成功啦！今后他qualified、rank或love或上传图后会主动{sub}。", messageObj);
            }

            if (UnsubscribeMapper != null)
            {
                if (messageObj.MessageType == MessageType.Group && messageObj.PermissionLevel == PermissionLevel.Public)
                    return new CommonMessageResponse(LoliReply.AdminOnly + "个人推送请私聊.", messageObj);

                int count = OldSiteApi.GetUser(UnsubscribeMapper, out var userObj);
                if (count == 0)
                {
                    return new CommonMessageResponse("找不到指定mapper..", messageObj);
                }

                if (count > 1)
                {
                    return new CommonMessageResponse($"找到{count}个mapper..", messageObj);
                }

                string mapperId = userObj.user_id;
                string mapperName = userObj.username;
                if (!_userDic.ContainsKey(mapperId) || !_userDic[mapperId].Contains(new UserInfo(subId, messageObj.MessageType)))
                {
                    string subject = messageObj.MessageType == MessageType.Private ? "你" : "本群";
                    return new CommonMessageResponse($"目前这个mapper没有被{subject}订阅..", messageObj);
                }

                _userDic[mapperId].Remove(new UserInfo(subId, messageObj.MessageType));
                if (_userDic[mapperId].Count == 0)
                    _userDic.Remove(mapperId, out _);

                SaveSettings(_userDic, "userDictionary");
                string sub = messageObj.MessageType == MessageType.Private ? "为你" : "在本群";
                return new CommonMessageResponse($"订阅取消成功，今后不再{sub}推送{mapperName}的有关动态。", messageObj);
            }

            return new CommonMessageResponse(LoliReply.ParamMissing, messageObj);
        }

        private static string[] GetSubscribed(MessageType messageType, string subId)
        {
            return _userDic.Where(k => k.Value.Contains(new UserInfo(subId, messageType))).Select(k => k.Key).ToArray();
        }

        private static void PushNews(IEnumerable<UserInfo> userList, IReadOnlyList<Beatmapsets> mapsets)
        {
            foreach (var userTuple in userList) // 遍历发给订阅此mapper的用户
            {
                var user = userTuple.User;
                var msgType = userTuple.MessageType;

                StringBuilder sb = new StringBuilder(mapsets[0].Creator + "有新的动态：\r\n");
                foreach (var mapset in mapsets)
                {
                    sb.AppendLine(string.Format(
                        "● {0}了{1} - {2} (https://osu.ppy.sh/beatmapsets/{3})",
                        mapset.Status == "pending" || mapset.Status == "wip" ? "上传" : mapset.Status,
                        mapset.Artist, mapset.Title, mapset.Id));
                }

                string userId = null, disscussId = null, groupId = null;
                switch (msgType)
                {
                    case MessageType.Private:
                        userId = user;
                        break;
                    case MessageType.Discuss:
                        disscussId = user;
                        break;
                    case MessageType.Group:
                        groupId = user;
                        break;
                }
#if DEBUG
                Logger.Success(sb.ToString().Trim('\n').Trim('\r'));
#else
                SendMessage(new CommonMessageResponse(sb.ToString().Trim('\n').Trim('\r'), userId, false), groupId,
                    disscussId, msgType);
                Thread.Sleep(3000);
#endif
            }
        }

        private void RemoveOverdueMapsets()
        {
            for (var i = 0; i < _todaySets.Count; i++)
            {
                var set = _todaySets[i];
                var duration = set.RankedDate != null
                    ? DateTime.UtcNow - set.RankedDate
                    : DateTime.UtcNow - set.SubmittedDate;
                if (duration <= RangeTime) // 若过期则删除记录
                    continue;
                _todaySets.Remove(set);
                SaveTodaySettings();
                i--;
            }
        }

        private Beatmapsets[] GetBeatmapsets(string mapperId)
        {
            var mapperName = OldSiteApi.GetUsernameByUid(mapperId);

            Beatmapsets[] mapsets = NewSiteApi
                .SearchAllBeatmaps(mapperName,
                    new BeatmapsetsSearchOptions { Status = BeatmapStatus.Qualified })
                .Union(NewSiteApi.SearchAllBeatmaps(mapperName,
                    new BeatmapsetsSearchOptions { Status = BeatmapStatus.PendingWip })).ToArray()
                .Union(NewSiteApi.SearchAllBeatmaps(mapperName)).ToArray();

            mapsets = mapsets
                .Where(i => i.Creator == mapperName)
                .Where(i =>
                    (i.Status == "qualified" || i.Status == "ranked" || i.Status == "loved") &&
                    DateTime.UtcNow - i.RankedDate < RangeTime ||
                    (i.Status == "pending" || i.Status == "wip") &&
                    DateTime.UtcNow - i.SubmittedDate < RangeTime).ToArray();

            // 先从今日已提醒中筛选此mapper的图
            IEnumerable<SlimBeatmapsets> todayThisCreatorSet = _todaySets.Where(k => k.Creator == mapperName);
            // 从总集合中筛选未提醒过的图
            IEnumerable<Beatmapsets> mapsetsNormal = mapsets.Where(set =>
                !todayThisCreatorSet.Select(todaySet => todaySet.Id).Contains(set.Id));
            // 从总集合中筛选提醒过，但是状态改变了的图
            Beatmapsets[] mapsetsStatusChanged = mapsets.Where(set =>
            {
                var matchedSet = todayThisCreatorSet.FirstOrDefault(k => k.Id == set.Id);
                return matchedSet != null && matchedSet.Status != set.Status;
            }).ToArray();
            // 若状态改变，删除之前的已保存信息
            if (mapsetsStatusChanged.Length > 0)
            {
                _todaySets.RemoveAll(s => mapsetsStatusChanged.Select(k => k.Id).Contains(s.Id));
                SaveTodaySettings();
            }

            // 重新集合
            mapsets = mapsetsNormal.Union(mapsetsStatusChanged).ToArray();
            return mapsets;
        }

        private void RemoveUnusefulMapper(string mapper)
        {
            _userDic.TryRemove(mapper, out _);
            SaveUserSettings();
        }

        private void SaveUserSettings() => SaveSettings(_userDic, "userDictionary");
        private void SaveTodaySettings() => SaveSettings(_todaySets, "today");

        public class SlimBeatmapsets
        {
            public string Id { get; set; }
            public DateTime? SubmittedDate { get; set; }
            public DateTime? RankedDate { get; set; }
            public string Creator { get; set; }
            public string Status { get; set; }
        }

        public struct UserInfo
        {
            public string User { get; set; }
            public MessageType MessageType { get; set; }

            public UserInfo(string user, MessageType messageType) : this()
            {
                User = user;
                MessageType = messageType;
            }
        }
    }
}
