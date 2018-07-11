using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Common.Assist;
using Daylily.Common.Database.BLL;
using Daylily.Common.Database.Model;
using Daylily.Common.Interface;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Models.Osu;
using Daylily.Common.Utils;
using Newtonsoft.Json;

namespace Daylily.Common.Function.Application.Command
{
    public class MyGraveyard : AppConstruct
    {
        public override string Name => "随机挖坑";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Beta;
        public override string VersionNumber => "1.0";
        public override string Description => "随机挖坑";
        public override string Command => "挖坑";
        public override AppType AppType => AppType.Command;

        private CommonMessage _message;
        private Thread _t;

        public override void OnLoad(string[] args)
        {
        }

        public override CommonMessageResponse OnExecute(in CommonMessage messageObj)
        {
            _message = messageObj;
            _t = new Thread(Async);
            _t.Start();
            return null;
        }

        private void Async()
        {
            try
            {
                BllUserRole bllUserRole = new BllUserRole();
                List<TblUserRole> userInfo = bllUserRole.GetUserRoleByQq(long.Parse(_message.UserId));
                if (userInfo.Count == 0)
                {
                    SendMessage(new CommonMessageResponse(LoliReply.IdNotBound, _message, true));
                    return;
                }

                var id = userInfo[0].UserId.ToString();

                List<Beatmap> totalList = new List<Beatmap>();
                Beatmap[] tmpArray;
                int page = 0;
                const int count = 10;
                do
                {
                    string json = WebRequestUtil.GetResponseString(
                        WebRequestUtil.CreateGetHttpResponse(
                            "https://osu.ppy.sh/users/" + id + "/beatmapsets/graveyard?offset=" + page + "&limit=" + count));
                    Logger.Debug("GET JSON");

                    tmpArray = JsonConvert.DeserializeObject<Beatmap[]>(json);
                    totalList.AddRange(tmpArray);
                    page += count;

                    if (tmpArray.Length != count) break;
                } while (tmpArray.Length != 0);

                if (totalList.Count == 0)
                {
                    SendMessage(new CommonMessageResponse("惊了，你竟然会没坑！", _message, true));
                    return;
                }

                Random rnd = new Random();
                Beatmap beatmap = totalList[rnd.Next(totalList.Count)];
                var cqMusic = new CustomMusic("https://osu.ppy.sh/s/" + beatmap.Id, $"https://b.ppy.sh/preview/{beatmap.Id}.mp3", beatmap.Title,
                    $"{beatmap.Artist}\r\n({beatmap.FavouriteCount} fav)", $"https://b.ppy.sh/thumb/{beatmap.Id}l.jpg");

                SendMessage(new CommonMessageResponse(cqMusic.ToString(), _message));
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

    }
}
