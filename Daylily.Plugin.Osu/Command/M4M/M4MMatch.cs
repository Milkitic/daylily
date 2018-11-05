using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bleatingsheep.Osu.ApiV2b.Models;
using Daylily.Bot;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Bot.Sessions;
using Daylily.Common;
using Daylily.Common.Utils.StringUtils;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;
using Daylily.Osu.Interface;
using Newtonsoft.Json;

namespace Daylily.Plugin.Osu
{
    [Name("m4m匹配")]
    [Author("yf_extension")]
    [Version(0, 2, 0, PluginVersion.Alpha)]
    [Help("这是一个利用平台进行自动化管理的M4M机制，无须自己各处求摸。",
        "将你自己的图上传，我会地合理地挑选与你和另一人互相推荐，以达成互相摸图的目的。",
        "每个人同时只限一张图，第二张图会覆盖第一张图。")]
    [Command("m4m")]
    public class M4MMatch : CommandPlugin
    {
        private static List<MatchInfo> _matchList;
        private CommonMessage _cm;
        private Session _session;
        private string _osuId;
        private MatchInfo _myInfo;

        private const int DefaultTimeout = 60000;

        [Arg("确认", IsSwitch = true, Default = false)]
        [Help("当匹配到的玩家向你提出审阅请求后，此选项可以完成审阅（务必经检查无误时再使用）。")]
        public bool Confirm { get; set; }

        [Arg("完成", IsSwitch = true, Default = false)]
        [Help("当发送者摸图完成时，此选项可以提醒匹配到的玩家审阅。")]
        public bool Finish { get; set; }

        [Arg("取消", IsSwitch = true, Default = false)]
        [Help("当双方进行了一周的匹配却仍然未完成时，此选项可以强制取消。")]
        public bool Cancel { get; set; }

        [Arg("l", IsSwitch = true, Default = false)]
        [Help("查看目前的列表。")]
        public bool List { get; set; }

        public override void Initialize(string[] args)
        {
            _matchList = LoadSettings<List<MatchInfo>>("MatchList") ?? new List<MatchInfo>();
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            _cm = messageObj;
            if (List)
            {
                if (_cm.PermissionLevel == PermissionLevel.Root)
                {
                    BllUserRole bllUserRole = new BllUserRole();
                    List<string> strs = new List<string>();
                    foreach (var item in _matchList)
                    {
                        var userInfo = bllUserRole.GetUserRoleByQq(long.Parse(item.Qq))[0];
                        string osuName = userInfo.CurrentUname;
                        string id = item.SetId;
                        string status;

                        if (item.TargetQq == null)
                            status = "等待匹配";
                        else
                        {
                            var info = bllUserRole.GetUserRoleByQq(long.Parse(item.TargetQq))[0];
                            string name = info.CurrentUname;
                            status = $"已经和{name}匹配";
                        }

                        if (id == null)
                            strs.Add(osuName + ": 闲置中。");
                        else
                            strs.Add($"{osuName}  地图号: s/{id} {status}");
                    }

                    return new CommonMessageResponse(string.Join("\r\n", strs), _cm);
                }
                else
                    return new CommonMessageResponse(LoliReply.RootOnly, _cm);
            }

            if (_cm.MessageType != MessageType.Private)
                return new CommonMessageResponse(LoliReply.PrivateOnly, _cm);

            try
            {
                using (_session = new Session(DefaultTimeout, _cm.Identity, _cm.UserId))
                {
                    BllUserRole bllUserRole = new BllUserRole();
                    List<TblUserRole> userInfo = bllUserRole.GetUserRoleByQq(long.Parse(messageObj.UserId));
                    if (userInfo.Count == 0)
                        return new CommonMessageResponse("这个功能是需要绑定osu id的，请使用/setid完成绑定", messageObj, true);

                    _osuId = userInfo[0].UserId.ToString();

                    try
                    {
                        // Init
                        if (!_matchList.Select(q => q.Qq).Contains(_cm.UserId))
                        {
                            const string intro = @"这是一个利用我进行自动化管理的M4M平台，只需将图的链接给我，你无需自己各处求摸。
我会地合理地从库中挑选，并与你和另一人互相推荐，以达成自动匹配M4M的目的。
每个人只能给我一张图，第二张图会覆盖第一张图，所以有什么图需摸要记得更新哦！
最重要的一点：这仅仅是一个平台，真正的交流还需面对面进行。";
                            SendMessage(new CommonMessageResponse(intro, _cm));

                            _matchList.Add(new MatchInfo(_cm.UserId));
                            SaveMatchList(); //apply

                            Thread.Sleep(8000);
                        }

                        // Load
                        _myInfo = _matchList.FirstOrDefault(q => q.Qq == _cm.UserId);
                        _myInfo.IsOperating = true;
                        _myInfo.LastUse = DateTime.Now;
                        if (PluginManager.ApplicationList.FirstOrDefault(o => o.GetType() == typeof(M4MMatchNotice)) is M4MMatchNotice plugin)
                        {
                            if (M4MMatchNotice.Tipped.ContainsKey(_myInfo.Qq))
                                M4MMatchNotice.Tipped[_myInfo.Qq] = DateTime.Now;
                            else
                                M4MMatchNotice.Tipped.TryAdd(_myInfo.Qq, DateTime.Now);
                            plugin.SaveSettings();
                        }
                        // Confirm
                        if (Confirm)
                        {
                            if (_myInfo.TargetQq == null)
                            {
                                return new CommonMessageResponse("你尚且还没有正在进行的匹配。", _cm);
                            }

                            var oInfo = _matchList.FirstOrDefault(q => q.Qq == _myInfo.TargetQq);
                            if (DateTime.Now - oInfo.LastConfirmedTime < new TimeSpan(3, 0, 0))
                                return new CommonMessageResponse("你已在三小时之内发出过核对确认，请稍后再试。", _cm);
                            oInfo.RequestBeConfirmed();
                            SaveMatchList(); //apply 

                            string nick = CqApi.GetStrangerInfo(oInfo.Qq).Data?.Nickname ?? "玩家";
                            SendMessage(new CommonMessageResponse($"你已经确认了{nick}({oInfo.Qq})的摸，请及时完成自己的摸！", _cm));

                            string nick2 = CqApi.GetStrangerInfo(_myInfo.Qq).Data?.Nickname ?? "玩家";
                            SendMessage(new CommonMessageResponse($"{nick2}({_myInfo.Qq})已经确认查收了你的摸。",
                                new Identity(oInfo.Qq, MessageType.Private)));

                            if (oInfo.LastConfirmedTime == null || _myInfo.LastConfirmedTime == null)
                                return null;
                            Thread.Sleep(2000);
                            oInfo.FinishedSet.Add(_myInfo.SetId);
                            _myInfo.FinishedSet.Add(oInfo.SetId);
                            oInfo.Finish();
                            _myInfo.Finish();
                            SendMessage(new CommonMessageResponse(
                                $"你与{nick}({oInfo.Qq})的M4M ({oInfo.SetUrl}) 已完成，合作愉快！",
                                new Identity(_myInfo.Qq, MessageType.Private)));
                            SendMessage(new CommonMessageResponse(
                                $"你与{nick2}({_myInfo.Qq})的M4M ({_myInfo.SetUrl}) 已完成，合作愉快！",
                                new Identity(oInfo.Qq, MessageType.Private)));
                            SaveMatchList(); //apply 
                            return null;
                        }
                        // Finish
                        else if (Finish)
                        {
                            var oInfo = _matchList.FirstOrDefault(q => q.Qq == _myInfo.TargetQq);
                            if (DateTime.Now - _myInfo.LastNoticeTime < new TimeSpan(3, 0, 0))
                                return new CommonMessageResponse("你已在三小时之内发出过完成提醒，请稍后再试。", _cm);
                            _myInfo.RequestFinishMatch();
                            SaveMatchList(); //apply 

                            string nick2 = CqApi.GetStrangerInfo(_myInfo.Qq).Data?.Nickname ?? "玩家";
                            SendMessage(new CommonMessageResponse(
                                $"{nick2}({_myInfo.Qq})已经摸完了你的图：\r\n" +
                                $"{oInfo.SetUrl}\r\n" +
                                $"正在等待你的摸。\r\n" +
                                $"请检查他的modding情况，回复 \"/m4m -确认\" 查收。\r\n" +
                                $"若对方没有摸完，请不要查收。若有疑问请相互交流。",
                                new Identity(oInfo.Qq, MessageType.Private)));
                            return new CommonMessageResponse("已发送完成请求。", _cm);
                        }
                        // Cancel
                        else if (Cancel)
                        {
                            var oInfo = _matchList.FirstOrDefault(q => q.Qq == _myInfo.TargetQq);

                            if (oInfo.MatchTime == null)
                                oInfo.MatchTime = DateTime.Now; // 临时补救

                            if (DateTime.Now - oInfo.MatchTime < new TimeSpan(7, 0, 0, 0))
                                return new CommonMessageResponse("取消失败，仅匹配持续一周以上才可取消。", _cm);

                            string nick = CqApi.GetStrangerInfo(oInfo.Qq).Data?.Nickname ?? "玩家";
                            string nick2 = CqApi.GetStrangerInfo(_myInfo.Qq).Data?.Nickname ?? "玩家";

                            oInfo.FinishedSet.Add(_myInfo.SetId);
                            _myInfo.FinishedSet.Add(oInfo.SetId);
                            oInfo.Finish();
                            _myInfo.Finish();
                            SendMessage(new CommonMessageResponse(
                                $"你与{nick}({oInfo.Qq})的M4M ({oInfo.SetUrl}) 已强制取消。",
                                new Identity(_myInfo.Qq, MessageType.Private)));
                            SendMessage(new CommonMessageResponse(
                                $"你与{nick2}({_myInfo.Qq})的M4M ({_myInfo.SetUrl}) 已强制取消。",
                                new Identity(oInfo.Qq, MessageType.Private)));
                            SaveMatchList(); //apply 
                            return null;
                        }

                        // No set after load
                        if (_myInfo.Set == null)
                            return SessionNoMap();

                        // Matched Main
                        string info;
                        if (_myInfo.TargetQq != null)
                        {
                            var oInfo = _matchList.FirstOrDefault(q => q.Qq == _myInfo.TargetQq);
                            string nick = CqApi.GetStrangerInfo(oInfo.Qq).Data?.Nickname ?? "玩家";
                            info = $"你已经成功和{nick}({oInfo.Qq})匹配\r\n" +
                                   $"· 你的信息：\r\n" +
                                   $"    地图地址：{_myInfo.SetUrl}\r\n" +
                                   $"    备注：{_myInfo.Mark}\r\n" +
                                   $"· 他的信息：\r\n" +
                                   $"    地图地址：{oInfo.SetUrl} \r\n" +
                                   $"    备注：{oInfo.Mark}\r\n" +
                                   $"当你摸图完成时，请使用 \"/m4m -完成\" 提醒对方审阅。\r\n" +
                                   $"当对方向你提出审阅请求后，请使用 \"/m4m -确认\" 完成审阅。\r\n" +
                                   $"若匹配持续超过一周，可使用  \"/m4m -取消\" 强制取消，且下次不会匹配到此图。";
                            return new CommonMessageResponse(info, _cm);
                        }

                        // Main
                        info = $"· 你的信息：\r\n" +
                               $"    地图地址：{_myInfo.SetUrl} \r\n" +
                               $"    备注：{_myInfo.Mark}\r\n" +
                               $"· 从下面的选项中选择一个你想进行的操作：\r\n" +
                               $">【1】删除已发布的地图。\r\n" +
                               $">【2】管理摸图偏好（当前：{_myInfo.GetPreferenceString()}）。\r\n" +
                               $">【3】开始进行m4m匹配。";

                        SendMessage(new CommonMessageResponse(info, _cm));
                        CommonMessage cmMain = SessionCondition("1", "2", "3");
                        switch (cmMain.Message)
                        {
                            case "1":
                                SendMessage(new CommonMessageResponse("删除现有地图，确认吗？\r\n" +
                                                                      "【1】是 【2】否", _cm));
                                CommonMessage cmPub = SessionCondition("1", "2");
                                if (cmPub.Message == "1")
                                {
                                    _myInfo.RemoveSet();
                                    SaveMatchList(); //apply 
                                    return new CommonMessageResponse("删除成功。使用/m4m重新发布地图。", _cm);
                                }
                                else
                                    return new CommonMessageResponse("你已取消操作。", _cm);
                            case "2":
                                if (SessionMode(out var sessionNoMap)) return sessionNoMap;
                                return null;
                            case "3": // core
                                MatchInfo[] fullList = _matchList.Where(i =>
                                    i.Qq != _myInfo.Qq && i.TargetQq == null && i.Set != null && !i.IsOperating &&
                                    DateTime.Now - i.LastUse < new TimeSpan(7, 0, 0, 0)).ToArray();
                                if (fullList.Length == 0)
                                {
                                    return new CommonMessageResponse("目前没有可匹配的用户，也有可能是可匹配的用户同时在浏览m4m菜单。请等待他人匹配或稍后重试。", _cm);
                                }

                                MatchInfo[] canModList = fullList.Where(i =>
                                    i.Preference.Have(_myInfo.Modes) == 1 &&
                                    _myInfo.Preference.Have(i.Modes) == 1 &&
                                    !i.FinishedSet.Contains(_myInfo.SetId) &&
                                    !_myInfo.FinishedSet.Contains(i.SetId)).ToArray();
                                if (canModList.Length == 0)
                                {
                                    return new CommonMessageResponse("目前没有与你情况相符合的用户，请等待他人匹配或稍后重试。", _cm);
                                }

                                MatchInfo[] bestList = new MatchInfo[0];
                                int count = 0;
                                while (bestList.Length == 0 && count < 120)
                                {
                                    bestList = canModList.Where(i =>
                                        Math.Abs(_myInfo.GetPreferanceTotalLength(i) -
                                                 i.GetPreferanceTotalLength(_myInfo)) < count).ToArray();
                                    count += 10;
                                    Thread.Sleep(1);
                                }

                                MatchInfo matchInfo;

                                if (bestList.Length < 1)
                                {
                                    SendMessage(new CommonMessageResponse("目前没有最佳的匹配，继续尝试不佳的匹配吗？\r\n" +
                                                                          "【1】是 【2】否", _cm));
                                    CommonMessage cmContinue = SessionCondition("1", "2");
                                    if (cmContinue.Message == "1")
                                    {
                                        MatchInfo[] notBestList = new MatchInfo[0];
                                        int c = count;
                                        while (notBestList.Length == 0 && c < 10000)
                                        {
                                            notBestList = canModList.Where(i =>
                                                Math.Abs(_myInfo.GetPreferanceTotalLength(i) -
                                                         i.GetPreferanceTotalLength(_myInfo)) < c).ToArray();
                                            c += 50;
                                            Thread.Sleep(1);
                                        }

                                        matchInfo = notBestList[Rnd.Next(notBestList.Length)];
                                    }
                                    else
                                        return new CommonMessageResponse("你已取消操作。请等待他人匹配或稍后重试。", _cm);
                                }
                                else
                                {
                                    matchInfo = bestList[Rnd.Next(bestList.Length)];
                                }

                                var data1 = CqApi.GetStrangerInfo(matchInfo.Qq).Data;
                                var data2 = CqApi.GetStrangerInfo(_myInfo.Qq).Data;
                                string sub1 = "Ta", sub2 = "Ta";
                                string nick1 = "玩家", nick2 = "玩家";
                                if (data1 != null)
                                {
                                    if (data1.Sex == "male")
                                        sub1 = "他";
                                    else if (data1.Sex == "female")
                                        sub1 = "她";
                                    nick1 = data1.Nickname;
                                }

                                if (data2 != null)
                                {
                                    if (data2.Sex == "male")
                                        sub2 = "他";
                                    else if (data2.Sex == "female")
                                        sub2 = "她";
                                    nick2 = data2.Nickname;
                                }

                                _myInfo.Start(matchInfo.Qq);
                                matchInfo.Start(_myInfo.Qq);
                                SaveMatchList(); //apply

                                const string tip = "当你选择完成摸图时，对方会收到一条消息确认你的mod，反之亦然。" +
                                                   "当双方互批完成时，M4M成功结束！\r\n" +
                                                   "若有疑问，请与对方互相协商。详细情况请使用 \"/m4m\"。";

                                SendMessage(new CommonMessageResponse($"{nick2}({_myInfo.Qq}) 与你成功匹配\r\n" +
                                                                      $"{sub2}的地图：{_myInfo.SetUrl}\r\n" +
                                                                      $"{sub2}的备注：{_myInfo.Mark}\r\n" + tip,
                                    new Identity(matchInfo.Qq, MessageType.Private)));

                                return new CommonMessageResponse($"你与{nick1}({matchInfo.Qq}) 成功匹配\r\n" +
                                                                 $"{sub1}的地图：{matchInfo.SetUrl}\r\n" +
                                                                 $"{sub1}的备注：{matchInfo.Mark}\r\n" + tip, _cm);
                            default:
                                return new CommonMessageResponse("你开始了？走了.jpg", _cm);
                        }
                    }
                    catch (TimeoutException)
                    {
                        return new CommonMessageResponse("由于长时间未操作，已经自动取消m4m状态。", _cm);
                    }
                }
            }
            catch (NotSupportedException)
            {
                return new CommonMessageResponse("你已在进行m4m状态中。", messageObj, true);
            }
            finally
            {
                if (_myInfo != null)
                {
                    _myInfo.IsOperating = false;
                    SaveMatchList(); //apply
                }
            }
        }

        /// <summary>
        /// 没有地图会话
        /// </summary>
        private CommonMessageResponse SessionNoMap()
        {
            SendMessage(new CommonMessageResponse("你还没有发布任何一张图，需要现在发布吗？\r\n" + "【1】是 【2】否", _cm));
            CommonMessage cmPub = SessionCondition("1", "2");

            return cmPub.Message == "1"
                ? SessionAddMap()
                : new CommonMessageResponse("由于你没有发布地图，已退出m4m模式。", _cm);
        }

        /// <summary>
        /// 更改摸图偏好会话
        /// </summary>
        private bool SessionMode(out CommonMessageResponse sessionNoMap)
        {
            SendMessage(new CommonMessageResponse("请告诉我你的摸图偏好，可多选。\r\n" +
                                                  "如发送 \"013\" 即指接受std、taiko、mania的地图\r\n" +
                                                  "【0】osu!standard\r\n" +
                                                  "【1】osu!taiko\r\n" +
                                                  "【2】osu!catch\r\n" +
                                                  "【3】osu!mania", _cm));

            if (SessionMultiCondition(out char[] choices, '0', '1', '2', '3'))
            {
                for (int i = 0; i < _myInfo.Preference.Length; i++)
                    _myInfo.Preference[i] = false;

                foreach (var item in choices)
                {
                    _myInfo.Preference[int.Parse(item.ToString())] = true;
                }

                SaveMatchList(); //apply 
                SendMessage(new CommonMessageResponse($"你的摸图偏好已更新为：{_myInfo.GetPreferenceString()}", _cm));
            }
            else
            {
                sessionNoMap = new CommonMessageResponse("你已取消操作，操作未保存。", _cm);
                return true;
            }

            sessionNoMap = null;
            return false;
        }

        /// <summary>
        /// 添加地图会话
        /// </summary>
        private CommonMessageResponse SessionAddMap()
        {
            if (SessionMode(out var sessionNoMap)) return sessionNoMap;
            Thread.Sleep(2000);
            SendMessage(new CommonMessageResponse("请将你的地图链接发送给我 :)", _cm));
            _session.Timeout = 120000;
            Beatmapset set = null;
            bool valid = false;
            int retry = 0;
            while (!valid && retry < 3)
            {
                CommonMessage cmMap = _session.GetMessage();
                string url = cmMap.Message.Replace("\r\n", "");

                set = GetBeatmapset(url);

                if (set != null)
                {
                    if (OldSiteApi.GetUidByUsername(set.Creator) != _osuId)
                    {
                        SendMessage(new CommonMessageResponse("这个不是你的地图哦！必须是要你自己上传的地图。再发一个新的地址给我吧？", _cm));
                    }
                    else if (set.RankedDate != null)
                    {
                        SendMessage(new CommonMessageResponse("这张图已经rank咯！必须是非rank图。再发一个新的地址给我吧？", _cm));
                    }
                    else
                        valid = true;
                }
                else
                    SendMessage(new CommonMessageResponse("这个地址……好像不对吧？请重新发送一个正确的地图地址。", _cm));

                retry++;
            }

            if (!valid)
            {
                Thread.Sleep(2000);
                return new CommonMessageResponse("由于发送的地址多次无效，操作失败，已退出m4m模式。", _cm);
            }
            else
            {
                SendMessage(new CommonMessageResponse("OK，已就绪！最后再进行一些备注描述吧，" +
                                                      "当对方和你匹配成功时会附带这些备注消息。\r\n" +
                                                      "（请3分钟内发送，须小于100字，多出的会被截取。）", _cm));
                _session.Timeout = 180000;
                CommonMessage cmMark = _session.GetMessage();
                string mark = new string(cmMark.Message.Take(100).ToArray());
                string verify = $"地图地址：https://osu.ppy.sh/beatmapsets/{set.Id} \r\n" +
                                $"备注：{mark}\r\n" +
                                "确定以上信息，就这样发布吗？\r\n" +
                                "【1】是 【2】否";
                SendMessage(new CommonMessageResponse(verify, _cm));
                _session.Timeout = DefaultTimeout;

                CommonMessage cmVerify = SessionCondition("1", "2");
                if (cmVerify.Message == "1")
                {
                    _myInfo.UpdateSet(set, mark);
                    SaveMatchList(); //apply 
                    return new CommonMessageResponse("操作已成功，已经收录了你的地图！请等待他人匹配或立刻使用 \"/m4m\" 主动匹配。", _cm);
                }
                else
                {
                    return new CommonMessageResponse("操作失败，用户已取消。", _cm);
                }
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
                SendMessage
                (conditions.Length < 4
                    ? new CommonMessageResponse($"请回复 \"{string.Join("\", \"", conditions)}\" 其中一个。", _cm)
                    : new CommonMessageResponse($"请回复形如 \"{conditions[0]}\" 的选项。", _cm));
                cmPub = _session.GetMessage();
                retryCount++;
            }

            return cmPub;
        }

        /// <summary>
        /// 多选会话
        /// </summary>
        private bool SessionMultiCondition(out char[] choice, params char[] conditions)
        {
            _session.Timeout = 30000;
            CommonMessage cmPub = _session.GetMessage();
            int retryCount = 0;
            while (cmPub.Message.ToArray().Distinct().Any(c => !conditions.Contains(c)) && retryCount < 3)
            {
                SendMessage(new CommonMessageResponse("存在无效输入，请重新选择。", _cm));
                cmPub = _session.GetMessage();
                retryCount++;
            }

            choice = cmPub.Message.ToArray().Distinct().ToArray();
            return retryCount < 3;
        }

        private Beatmapset GetBeatmapset(string url)
        {
            StringFinder sf = new StringFinder(url);
            if (url.Length > 8 && sf.FindNext("ppy.sh/b/", false) != 8)
            {
                SendMessage(new CommonMessageResponse("请稍后，核对中……", _cm));
                sf.FindToLast();
                string cut = sf.Cut();
                string bId = cut.Split('?')[0];
                return NewSiteApi.GetBeatmapsetsByBidAsync(bId).Result;
            }

            if (url.Length > 8 && sf.FindNext("ppy.sh/s/", false) != 8)
            {
                SendMessage(new CommonMessageResponse("请稍后，核对中……", _cm));
                sf.FindToLast();
                string cut = sf.Cut();
                string sId = cut.Split('?')[0];
                return NewSiteApi.GetBeatmapsetsBySidAsync(sId).Result;
            }

            if (url.Length > 18 && sf.FindNext("ppy.sh/beatmapsets/", false) != 18)
            {
                SendMessage(new CommonMessageResponse("请稍后，核对中……", _cm));
                sf.FindToLast();
                string cut = sf.Cut();
                string sId = cut.Split('#')[0].Split('?')[0];
                return NewSiteApi.GetBeatmapsetsBySidAsync(sId).Result;
            }

            return null;
        }

        private void SaveMatchList()
        {
            SaveSettings(_matchList, "MatchList");
        }
    }
    public class LightMaps
    {
        public int TotalLength { get; set; }
        public string Mode { get; set; }
    }
    public class LightSets
    {
        public string Id { get; set; }
        public List<LightMaps> Beatmaps { get; set; }

    }

    public class MatchInfo
    {
        public string Qq { get; set; }
        public bool[] Preference { get; set; } = { true, false, false, false }; // 0=std, 1=taiko, 2=ctb, 3=mania
        public LightSets Set { get; set; }
        public string Mark { get; set; }
        public string TargetQq { get; set; }
        public DateTime? LastConfirmedTime { get; set; }
        public DateTime? LastNoticeTime { get; set; }
        public List<string> FinishedSet { get; set; } = new List<string>();
        public DateTime? MatchTime { get; set; }
        public DateTime LastUse { get; set; }
        [JsonIgnore] public bool IsOperating { get; set; } = false;

        // extension
        [JsonIgnore] public string SetId => Set?.Id;
        [JsonIgnore] public string SetUrl => Set == null ? null : "https://osu.ppy.sh/beatmapsets/" + Set.Id;
        [JsonIgnore] public int DiffCount => Set.Beatmaps.Count;
        [JsonIgnore] public int AvgLength => (int)Math.Round(Set.Beatmaps.Average(k => k.TotalLength));
        [JsonIgnore] public int TotalLength => Set.Beatmaps.Sum(k => k.TotalLength);

        [JsonIgnore]
        public bool[] Modes
        {
            get
            {
                bool[] modes = { false, false, false, false };
                foreach (var map in Set.Beatmaps)
                {
                    switch (map.Mode)
                    {
                        case "osu":
                            modes[0] = true;
                            break;
                        case "taiko":
                            modes[1] = true;
                            break;
                        case "fruits":
                            modes[2] = true;
                            break;
                        case "mania":
                            modes[3] = true;
                            break;
                    }
                }

                return modes;
            }
        }

        public void UpdateSet(Beatmapset set, string mark)
        {
            Contract.Requires<InvalidOperationException>(TargetQq == null);
            LightSets tmpSets = new LightSets
            {
                Id = set.Id,
                Beatmaps = new List<LightMaps>()
            };

            foreach (var s in set.Beatmaps)
            {
                tmpSets.Beatmaps.Add(new LightMaps
                {
                    TotalLength = s.TotalLength,
                    Mode = s.Mode
                });
            }

            Set = tmpSets;
            Mark = mark;
        }

        public void UpdateSet(string setId, string mark)
        {
            Contract.Requires<InvalidOperationException>(TargetQq == null);
            UpdateSet(NewSiteApi.GetBeatmapsetsBySidAsync(setId).Result, mark);
        }

        public void RemoveSet()
        {
            Contract.Requires<InvalidOperationException>(TargetQq == null);
            Set = null;
            Mark = null;
        }

        public void Start(string targetQq)
        {
            Contract.Requires<InvalidOperationException>(TargetQq != targetQq);
            TargetQq = targetQq;
            LastNoticeTime = null;
            LastConfirmedTime = null;
            MatchTime = DateTime.Now;
        }

        public void RequestFinishMatch()
        {
            LastNoticeTime = DateTime.Now;
        }

        public void RequestBeConfirmed()
        {
            LastConfirmedTime = DateTime.Now;
        }

        public void Finish()
        {
            TargetQq = null;
            LastNoticeTime = null;
            LastConfirmedTime = null;
            MatchTime = null;
        }

        public MatchInfo()
        {
        }

        public MatchInfo(string qq)
        {
            Qq = qq;
        }

        public string GetPreferenceString()
        {
            List<string> modes = new List<string>();
            for (var i = 0; i < 4; i++)
            {
                if (!Preference[i]) continue;
                switch (i)
                {
                    case 0:
                        modes.Add("std");
                        break;
                    case 1:
                        modes.Add("taiko");
                        break;
                    case 2:
                        modes.Add("ctb");
                        break;
                    case 3:
                        modes.Add("mania");
                        break;
                }
            }

            return string.Join(", ", modes);
        }
    }
}

