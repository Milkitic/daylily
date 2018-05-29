using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.CosResponse;
using System.Collections.Generic;
using System.Linq;
using Daylily.Common.Interface.CQHttp;

namespace Daylily.Web.Function.Application
{
    public class PornDetector : AppConstruct
    {
        private static Dictionary<string, int> UserCount { get; } = new Dictionary<string, int>();
        private static Dictionary<string, CosObject> Md5List { get; } = new Dictionary<string, CosObject>();

        public override CommonMessageResponse Execute(CommonMessage message)
        {
            // 查黄图
            if (message.Group != null && message.GroupId != "133605766") return null;

            //if (user != "2241521134") return null;
            var imgList = CqCode.GetImageInfo(message.Message);
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

            Logger.WarningLine("发现了" + (urlList.Count + cacheList.Count) + "张图");

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

                var abc = WebRequestHelper.CreatePostHttpResponse("http://service.image.myqcloud.com/detection/porn_detect", str, authorization: Signature.Get());
                var respStr = WebRequestHelper.GetResponseString(abc);

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
                    case 0 when item.data.normal_score > item.data.hot_score && item.data.normal_score > item.data.porn_score && item.data.confidence > 40:
                        continue;
                    case 1:
                    case 2:
                        CqApi.SetGroupBan(message.GroupId, message.UserId, 24 * 60 * 60);
                        return new CommonMessageResponse("……………………");
                }

                if (item.data.porn_score >= item.data.hot_score && item.data.porn_score > 65)
                {
                    return AddCount(message.UserId, message.GroupId);
                }

                if (item.data.hot_score >= item.data.porn_score && item.data.hot_score > item.data.normal_score && item.data.hot_score > 80)
                {
                    return AddCount(message.UserId, message.GroupId);
                }
                break;
            }
            return null;
        }

        private CommonMessageResponse AddCount(string user, string group)
        {
            Logger.WarningLine("发现好图，存了");
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
    }
}
