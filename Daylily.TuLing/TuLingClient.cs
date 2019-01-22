using Daylily.Common.Web;
using Daylily.TuLing.RequestModel;
using Daylily.TuLing.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Daylily.TuLing
{
    public class TuLingClient
    {
        private const string RequestUri = "http://openapi.tuling123.com/openapi/api/v2";

        public Response SendText(string content, string userId, string groupId, string userName = null)
        {
            var request = new Request
            {
                Perception = new Perception
                {
                    InputText = new InputText
                    {
                        Text = content
                    }
                },
                UserInfo = new UserInfo
                {
                    ApiKey = TuLingSecret.ApiKey,
                    UserId = userId,
                    GroupId = groupId,
                    UserIdName = userName
                }
            };

            var json = HttpClient.HttpPost(RequestUri, JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Response>(json);
        }
    }
}
