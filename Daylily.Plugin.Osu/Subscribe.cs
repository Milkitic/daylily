using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Common.Logging;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using Daylily.Osu;
using OSharp.V1.Beatmap;
using OSharp.V1.User;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Osu
{
    [Name("Mapper订阅")]
    [Author("yf_extension")]
    [Version(2, 1, 3, PluginVersion.Alpha)]
    [Help("订阅某个mapper的qua、rank、love提醒。", "限制为群内推送10个名额，个人推送5个名额。")]
    [Command("sub")]
    public class Subscribe : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("2690bff7-1e5c-4069-98b8-d0fdfc612a02");

        [Arg("u")]
        [Help("需要取消订阅的mapper用户名。")]
        public string UnsubscribeMapper { get; set; }

        [Arg("l", IsSwitch = true)]
        [Help("若启用，查询已经订阅的mapper。")]
        public bool List { get; set; }

        [FreeArg]
        [Help("需要订阅的mapper用户名。")]
        public string SubscribeMapper { get; set; }

        private static ConcurrentDictionary<long, List<CoolQIdentity>> _userDic;
        private static List<SlimBeatmapsets> _todaySets;

        private static readonly TimeSpan RangeTime = new TimeSpan(24, 0, 0);
        private static OldSiteApiClient _client;
        private const int PrivateMax = 5;
        private const int GroupMax = 10;

        public class Test
        {
            public string Id { get; set; }
            public MessageType Type { get; set; }
        }

        public override void OnInitialized(StartupConfig startup)
        {
            _userDic = LoadSettings<ConcurrentDictionary<long, List<CoolQIdentity>>>("userDictionary") ??
                       new ConcurrentDictionary<long, List<CoolQIdentity>>();
            _todaySets = LoadSettings<List<SlimBeatmapsets>>("today") ?? new List<SlimBeatmapsets>();
            _client = new OldSiteApiClient();

            Task.Run(() =>
            {
                // 扫描的频率，但未包括等待另一线程的时间，也就是>=这个间隔。
#if DEBUG
                const int sleepTime = 5000;
#else
                const int sleepTime = 1000 * 60 * 20;
#endif

                while (true)
                {
                    // 由于其中会有很多阻塞 使用异步
                    Task asyncTask = new Task(() =>
                    {
                        RemoveOverdueMapSets();

                        // 以mapper为单位的设计
                        foreach (var pair in _userDic)
                        {
                            try
                            {
                                long mapperId = pair.Key;
                                List<CoolQIdentity> userList = pair.Value;
                                if (pair.Value.Count == 0)
                                {
                                    RemoveUnusefulMapper(mapperId);
                                    continue;
                                }

                                OsuBeatmapSet[] mapsets = GetBeatmapSets(new UserId(mapperId));

                                if (mapsets.Length <= 0) continue;
                                foreach (var item in mapsets)
                                {
                                    _todaySets.Add(new SlimBeatmapsets
                                    {
                                        CreatorId = mapperId,
                                        Id = item.Id,
                                        RankedDate = item.RankedDate,
                                        Status = item.Status,
                                        //SubmittedDate = item.SubmittedDate
                                    });
                                }

                                SaveTodaySettings();
                                Logger.Debug($"{pair.Key}: Find {mapsets.Length} results.");
                                PushNews(userList, mapsets);

                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex);
                            }
                        }
                    });

                    asyncTask.Start();
                    Thread.Sleep(sleepTime);
                    Task.WaitAll(asyncTask); // 等待执行完毕
                }
            });
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (List)
            {
                UserId[] subscribedUsers = GetSubscribed(routeMsg.CoolQIdentity)
                    .Select(k => new UserId(k))
                    .OrderBy(k => k.IdOrName)
                    .ToArray();
                int count = subscribedUsers.Length;
                string subject = routeMsg.MessageType == MessageType.Private ? "你" : "本群";
                if (count == 0)
                    return routeMsg.ToSource($"{subject}没有订阅任何mapper。");
                var names = _client.GetUserNameByUid(subscribedUsers);
                return routeMsg.ToSource($"以下是{subject}的订阅名单，共计{count}个：\r\n{string.Join("\r\n", names)}");
            }

            if (SubscribeMapper != null)
            {
                if (routeMsg.MessageType == MessageType.Group && routeMsg.CurrentAuthority == Authority.Public)
                    return routeMsg.ToSource(DefaultReply.AdminOnly + "个人推送请私聊.");

                var subscribedUsers = GetSubscribed(routeMsg.CoolQIdentity)
                    .Select(k => new UserId(k))
                    .OrderBy(k => k.IdOrName)
                    .ToArray();
                int subedCount = subscribedUsers.Length;
                if (routeMsg.MessageType == MessageType.Private && subedCount >= PrivateMax)
                {
                    return routeMsg.ToSource(
                        string.Format("你已经订阅了{0}个mapper啦，人家已经装不下的说：{1}", subedCount,
                            string.Join(", ", _client.GetUserNameByUid(subscribedUsers))));
                }

                if (routeMsg.MessageType != MessageType.Private && subedCount >= GroupMax)
                {
                    return routeMsg.ToSource(
                        string.Format("这个群已经订阅了{0}个mapper啦，人家已经装不下的说：{1}", subedCount,
                            string.Join(", ", _client.GetUserNameByUid(subscribedUsers))));
                }

                int count = _client.GetUser(UserComponent.FromUserName(SubscribeMapper), out var userObj);
                if (count == 0)
                    return routeMsg.ToSource("找不到指定mapper..");

                if (count > 1)
                    return routeMsg.ToSource($"找到{count}个mapper..");

                long mapperId = userObj.UserId;
                string mapperName = userObj.UserName;

                if (!_userDic.ContainsKey(mapperId))
                    _userDic.TryAdd(mapperId, new List<CoolQIdentity>());
                if (_userDic[mapperId].Contains(routeMsg.CoolQIdentity))
                {
                    string subject = routeMsg.MessageType == MessageType.Private ? "你" : "本群";
                    return routeMsg.ToSource($"{subject}已经订阅过{mapperName}啦..");
                }

                _userDic[mapperId].Add(routeMsg.CoolQIdentity);
                SaveSettings(_userDic, "userDictionary");
                string sub = routeMsg.MessageType == MessageType.Private ? "私聊提醒你" : "在本群提醒";
                return routeMsg.ToSource($"{mapperName}订阅成功啦！今后他qualified、rank或love或上传图后会主动{sub}。");
            }

            if (UnsubscribeMapper != null)
            {
                if (routeMsg.MessageType == MessageType.Group && routeMsg.CurrentAuthority == Authority.Public)
                    return routeMsg.ToSource(DefaultReply.AdminOnly + "个人推送请私聊.");

                int count = _client.GetUser(UserComponent.FromUserName(UnsubscribeMapper), out var userObj);
                if (count == 0)
                {
                    return routeMsg.ToSource("找不到指定mapper..");
                }

                if (count > 1)
                {
                    return routeMsg.ToSource($"找到{count}个mapper..");
                }

                long mapperId = userObj.UserId;
                string mapperName = userObj.UserName;
                if (!_userDic.ContainsKey(mapperId) || !_userDic[mapperId].Contains(routeMsg.CoolQIdentity))
                {
                    string subject = routeMsg.MessageType == MessageType.Private ? "你" : "本群";
                    return routeMsg.ToSource($"目前这个mapper没有被{subject}订阅..");
                }

                _userDic[mapperId].Remove(routeMsg.CoolQIdentity);
                if (_userDic[mapperId].Count == 0)
                    _userDic.Remove(mapperId, out _);

                SaveSettings(_userDic, "userDictionary");
                string sub = routeMsg.MessageType == MessageType.Private ? "为你" : "在本群";
                return routeMsg.ToSource($"订阅取消成功，今后不再{sub}推送{mapperName}的有关动态。");
            }

            return routeMsg.ToSource(DefaultReply.ParamMissing);
        }

        private static IEnumerable<long> GetSubscribed(CoolQIdentity identity)
        {
            return _userDic.Where(k => k.Value.Contains(identity)).Select(k => k.Key);
        }

        private void PushNews(IEnumerable<CoolQIdentity> userList, IReadOnlyList<OsuBeatmapSet> mapsets)
        {
            foreach (var identity in userList) // 遍历发给订阅此mapper的用户
            {
                StringBuilder sb = new StringBuilder(mapsets[0].Creator + "有新的动态：\r\n");
                foreach (var mapset in mapsets)
                {
                    sb.AppendLine(string.Format(
                        "・{0}了{1} - {2} (https://osu.ppy.sh/beatmapsets/{3})", StatusToReadable(mapset.Status),
                        mapset.Artist, mapset.Title, mapset.Id));
                }

                var str = sb.ToString().Trim('\n').Trim('\r');
                Logger.Success(str);

#if DEBUG

#else
                SaveLogs(str, "pushes");
                SendMessageAsync(new CoolQRouteMessage(str, identity));
                Thread.Sleep(3000);
#endif

            }
            string StatusToReadable(BeatmapApprovedState? status)
            {
                switch (status)
                {
                    case BeatmapApprovedState.Graveyard:
                        return "坟";
                    case BeatmapApprovedState.Pending:
                        return "上传";
                    case BeatmapApprovedState.Ranked:
                        return "Rank";
                    case BeatmapApprovedState.Approved:
                        return "Approve";
                    case BeatmapApprovedState.Qualified:
                        return "Qualify";
                    case BeatmapApprovedState.Loved:
                        return "Love";
                    case null:
                        return " *不可描述* ";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(status), status, null);
                }
            }
        }

        private void RemoveOverdueMapSets()
        {
            for (var i = 0; i < _todaySets.Count; i++)
            {
                var set = _todaySets[i];
                //var duration = set.RankedDate != null
                //    ? DateTime.UtcNow - set.RankedDate
                //    : DateTime.UtcNow - set.SubmittedDate;
                var duration = DateTime.UtcNow - set.RankedDate;
                if (duration <= RangeTime) // 若过期则删除记录
                    continue;
                _todaySets.Remove(set);
                SaveTodaySettings();
                i--;
            }
        }

        private OsuBeatmapSet[] GetBeatmapSets(UserComponent user)
        {
            _client.GetUser(user, out var userObj);

            //OsuBeatmapSet[] mapsets = NewSiteApiClient
            //    .SearchAllBeatmaps(mapperName, new BeatmapsetsSearchOptions { Status = BeatmapStatus.Qualified })
            //    .Union(
            //        NewSiteApiClient.SearchAllBeatmaps(
            //            mapperName,
            //            new BeatmapsetsSearchOptions { Status = BeatmapStatus.PendingWip }
            //        )
            //    )
            //    .ToArray()
            //    .Union(NewSiteApiClient.SearchAllBeatmaps(mapperName))
            //    .ToArray();
            OsuBeatmapSet[] mapsets = _client
                .GetBeatmapSetsByCreator(user)
                .ToArray();
            mapsets = mapsets
                .Where(i => i.Creator == userObj.UserName)
                .Where(i =>
                    (i.Status == BeatmapApprovedState.Qualified
                     || i.Status == BeatmapApprovedState.Ranked
                     || i.Status == BeatmapApprovedState.Loved) &&
                    i.RankedDate != null &&
                    DateTime.UtcNow - i.RankedDate < RangeTime /*||
                    (i.Status == "pending" || i.Status == "wip") &&
                    DateTime.UtcNow - i.SubmittedDate < RangeTime*/).ToArray();

            // 先从今日已提醒中筛选此mapper的图 IEnumerable<SlimBeatmapsets>
            var todayThisCreatorSet = _todaySets.Where(k => k.CreatorId == userObj.UserId);
            // 从总集合中筛选未提醒过的图 IEnumerable<OsuBeatmapSet>
            var mapsetsNormal = mapsets.Where(set =>
                !todayThisCreatorSet.Select(todaySet => todaySet.Id).Contains(set.Id));

            // 从总集合中筛选提醒过，但是状态改变了的图
            OsuBeatmapSet[] mapsetsStatusChanged = mapsets.Where(set =>
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

        private void RemoveUnusefulMapper(long creatorId)
        {
            _userDic.TryRemove(creatorId, out _);
            SaveUserSettings();
        }

        private void SaveUserSettings() => SaveSettings(_userDic, "userDictionary");
        private void SaveTodaySettings() => SaveSettings(_todaySets, "today");

        public class SlimBeatmapsets
        {
            public long Id { get; set; }
            //public DateTime? SubmittedDate { get; set; }
            public DateTimeOffset? RankedDate { get; set; }
            public long CreatorId { get; set; }
            public BeatmapApprovedState? Status { get; set; }
        }
    }
}
