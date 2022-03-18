using daylily.ThirdParty.Tuling.RequestModel;
using daylily.ThirdParty.Tuling.ResponseModel;
using MilkiBotFramework.Connecting;

namespace daylily.ThirdParty.Tuling
{
    public class TulingClient
    {
        private readonly LightHttpClient _lightHttpClient;

        public TulingClient(LightHttpClient lightHttpClient)
        {
            _lightHttpClient = lightHttpClient;
        }

        private const string RequestUri = "http://openapi.tuling123.com/openapi/api/v2";

        public async Task<Response> SendText(string apiKey, string content, string userId, string groupId, string? userName = null)
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
                    ApiKey = apiKey,
                    UserId = userId,
                    GroupId = groupId,
                    UserIdName = userName
                }
            };

            return await _lightHttpClient.HttpPost<Response>(RequestUri, request);
        }
    }
}
