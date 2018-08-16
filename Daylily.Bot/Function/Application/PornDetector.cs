using System.Collections.Generic;
using System.Linq;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.Models.CosResponse;
using Daylily.Bot.PluginBase;
using Daylily.Common.Utils.HttpRequest;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.Cos;

namespace Daylily.Bot.Function.Application
{
    [Name("嗅探黄图")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Beta)]
    [Help("发现福利图和黄图时进行提醒和禁言（仅新人mapper群）")]
    public class PornDetector : ApplicationPlugin
    {
        private static Dictionary<string, int> UserCount { get; set; } = new Dictionary<string, int>();
        private static Dictionary<string, CosObject> Md5List { get; } = new Dictionary<string, CosObject>();

        public PornDetector()
        {
            Logger.Origin("上次用户计数载入中。");
            UserCount = LoadSettings<Dictionary<string, int>>() ?? new Dictionary<string, int>();
            Logger.Origin("上次用户计数载入完毕。");
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            // 查黄图
            if (messageObj.Group == null || messageObj.GroupId != "133605766") return null;
            var imgList = CqCode.GetImageInfo(messageObj.Message);
            if (imgList == null)
                return null;
            List<string> urlList = new List<string>();
            List<CosObject> cacheList = new List<CosObject>();
            foreach (var item in imgList)
            {
                if (Md5List.Keys.Contains(item.Md5))
                    cacheList.Add(Md5List[item.Md5]);
                else if (item.Size > 1000 * 60) //60KB
                    urlList.Add(item.Url);
            }

            if (urlList.Count == 0 && cacheList.Count == 0)
                return null;

            Logger.Warn("发现了" + (urlList.Count + cacheList.Count) + "张图");

            CosAnalyzer model = new CosAnalyzer
            {
                result_list = new List<CosObject>()
            };

            if (urlList.Count != 0)
            {
                string str = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    appid = "1252749411",
                    url_list = urlList.ToArray()
                });

                var abc = WebRequestUtil.CreatePostHttpResponse(
                    "http://service.image.myqcloud.com/detection/porn_detect", str, authorization: Signature.Get());
                var respStr = WebRequestUtil.GetResponseString(abc);

                model = Newtonsoft.Json.JsonConvert.DeserializeObject<CosAnalyzer>(respStr);
            }

            model.result_list.AddRange(cacheList);
            int i = 0;
            foreach (var item in model.result_list)
            {
                if (i < imgList.Length && !Md5List.Keys.Contains(imgList[i].Md5))
                    Md5List.Add(imgList[i].Md5, item);
                i++;

                switch (item.data.result)
                {
                    case 0 when item.data.normal_score > item.data.hot_score &&
                                item.data.normal_score > item.data.porn_score && item.data.confidence > 40:
                        continue;
                    case 1:
                    case 2:
                        CqApi.SetGroupBan(messageObj.GroupId, messageObj.UserId, 24 * 60 * 60);
                        return new CommonMessageResponse("……………………");
                    default:
                        break;
                }

                if (item.data.porn_score >= item.data.hot_score && item.data.porn_score > 65)
                    return AddCount(messageObj.UserId, messageObj.GroupId);

                if (item.data.hot_score >= item.data.porn_score && item.data.hot_score > item.data.normal_score &&
                    item.data.hot_score > 80)
                    return AddCount(messageObj.UserId, messageObj.GroupId);

                break;
            }

            return null;

            //if (user != "2241521134") return null;
        }

        private CommonMessageResponse AddCount(string user, string group)
        {
            try
            {
                Logger.Warn("发现好图，存了");
                if (!UserCount.ContainsKey(user))
                    UserCount.Add(user, 2);
                UserCount[user]--;
                if (UserCount[user] != 0)
                    return new CommonMessageResponse("..黄花菜看了都脸红..求你少发点", user, true);
                else
                {
                    UserCount[user] = 2;
                    CqApi.SetGroupBan(group, user, (int)(0.5 * 60 * 60));
                    return new CommonMessageResponse("...", user, true);
                }
            }
            finally
            {
                SaveSettings(UserCount);
            }
        }
    }
}
