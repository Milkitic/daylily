using DaylilyWeb.Assist;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions.Applications
{
    public class ImageAnalyzer : Application
    {
        HttpApi CQApi = new HttpApi();

        public ImageAnalyzer()
        {
            appType = AppType.Public;
        }

        public override string Execute(string message, string user, string group, bool isRoot, ref bool ifAt)
        {
            // 查黄图
            if (group != "133605766" && user != "2241521134") return null;
            string[] imgs = CQCode.GetImageUrls(message);
            if (imgs == null)
                return null;
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                appid = "1252749411",
                url_list = imgs
            });

  
            var abc = WebRequestHelper.CreatePostHttpResponse("http://service.image.myqcloud.com/detection/porn_detect", str, authorization: Signature.Get());
            var resp_str = WebRequestHelper.GetResponseString(abc);

            Models.CosResponse.CosAnalyzer resp_model = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.CosResponse.CosAnalyzer>(resp_str);
            foreach (var item in resp_model.result_list)
                if (item.data.result == 0 && item.data.normal_score > item.data.hot_score && item.data.normal_score > item.data.porn_score && item.data.confidence > 40)
                    continue;
                else
                {
                    if (item.data.result == 1 || item.data.result == 2)
                    {
                        CQApi.SetGroupBan(group, user, 24 * 60 * 60);
                        return "给我进去吧你";
                    }
                    else
                    {
                        if (item.data.porn_score >= item.data.hot_score && item.data.porn_score > 60)
                        {
                            //return "球球你，营养不够了";
                        }
                        if (item.data.hot_score >= item.data.porn_score && item.data.hot_score > item.data.normal_score && item.data.hot_score > 80)
                        {
                            //return "社保";
                        }
                        break;
                    }
                }
            return null;
        }
    }
}
