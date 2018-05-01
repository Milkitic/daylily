using DaylilyWeb.Assist;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using DaylilyWeb.Models.CosResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Function.Application
{
    public class PornDetector : Application
    {
        HttpApi CQApi = new HttpApi();

        private static Dictionary<string, int> UserCount { get; set; } = new Dictionary<string, int>();
        private static Dictionary<string, CosObject> MD5List { get; set; } = new Dictionary<string, CosObject>();

        public override string Execute(string message, string user, string group, PermissionLevel currentLevel, ref bool ifAt)
        {
            // 查黄图
            if (group != "133605766") return null;

            //if (user != "2241521134") return null;
            var img_list = CQCode.GetImageInfo(message);
            if (img_list == null)
                return null;
            List<string> url_list = new List<string>();
            List<CosObject> cache_list = new List<CosObject>();
            foreach (var item in img_list)
            {
                if (MD5List.Keys.Contains(item.Md5))
                    cache_list.Add(MD5List[item.Md5]);
                else if (item.Size > 1000 * 60) //60KB
                    url_list.Add(item.Url);
            }
            if (url_list.Count == 0 && cache_list.Count == 0)
                return null;

            Logger.WarningLine("发现了" + (url_list.Count + cache_list.Count) + "张图");

            CosAnalyzer model = new CosAnalyzer
            {
                result_list = new List<CosObject>()
            };

            if (url_list.Count != 0)
            {
                string str = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    appid = "1252749411",
                    url_list = url_list.ToArray()
                });

                var abc = WebRequestHelper.CreatePostHttpResponse("http://service.image.myqcloud.com/detection/porn_detect", str, authorization: Signature.Get());
                var resp_str = WebRequestHelper.GetResponseString(abc);

                model = Newtonsoft.Json.JsonConvert.DeserializeObject<CosAnalyzer>(resp_str);
            }

            model.result_list.AddRange(cache_list);
            int i = 0;
            foreach (var item in model.result_list)
            {
                if (i < img_list.Length && !MD5List.Keys.Contains(img_list[i].Md5))
                    MD5List.Add(img_list[i].Md5, item);
                i++;

                if (item.data.result == 0 && item.data.normal_score > item.data.hot_score && item.data.normal_score > item.data.porn_score && item.data.confidence > 40)
                    continue;
                else
                {
                    if (item.data.result == 1 || item.data.result == 2)
                    {
                        CQApi.SetGroupBan(group, user, 24 * 60 * 60);
                        return "……………………";
                    }
                    else
                    {
                        if (item.data.porn_score >= item.data.hot_score && item.data.porn_score > 65)
                        {
                            //ifAt = true;
                            return AddCount(user, group, ref ifAt);
                            //return "球球你，营养不够了";
                        }
                        if (item.data.hot_score >= item.data.porn_score && item.data.hot_score > item.data.normal_score && item.data.hot_score > 80)
                        {
                            //ifAt = true;
                            return AddCount(user, group, ref ifAt);
                            //return "社保";
                        }
                        break;
                    }
                }
            }
            return null;
        }

        private string AddCount(string user, string group, ref bool ifAt)
        {
            Logger.WarningLine("发现好图，存了");
            if (!UserCount.ContainsKey(user))
                UserCount.Add(user, 2);
            UserCount[user]--;
            if (UserCount[user] != 0)
            {
                ifAt = true;
                return "..黄花菜看了都脸红..求你少发点";
                //return "你还能发" + UserCount[user] + "张这样的图";
            }
            else
            {
                UserCount[user] = 2;
                CQApi.SetGroupBan(group, user, (int)(0.5 * 60 * 60));
                return "。。。";
            }
        }
    }
}
