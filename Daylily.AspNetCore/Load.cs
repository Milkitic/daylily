using Daylily.Bot;
using Daylily.Common;
using Daylily.Common.IO;
using Daylily.Common.Logging;
using Daylily.Common.Text;
using Daylily.CoolQ.CoolQHttp;
using Daylily.CoolQ.Message;
using Daylily.Cos;
using Daylily.Osu;
using Daylily.Osu.Data;
using System;
using System.IO;
using Daylily.TuLing;

namespace Daylily.AspNetCore
{
    internal static class Load
    {
        public static void LoadSecret()
        {
            var file = new FileInfo(Path.Combine(Domain.SecretPath, "secret.json"));
            string json;
            Secret secret;
            if (!file.Exists)
            {
                secret = new Secret();
                json = Newtonsoft.Json.JsonConvert.SerializeObject(secret);
                ConcurrentFile.WriteAllText(file.FullName, json.ToJsonFormat());

                Logger.Error("请完善secret配置。");
                Console.ReadKey();
                Environment.Exit(0);
            }

            json = ConcurrentFile.ReadAllText(file.FullName);
            secret = Newtonsoft.Json.JsonConvert.DeserializeObject<Secret>(json);

            // 读设置
            DbHelper.ConnectionString.Add("cabbage", secret.ConnectionStrings.DefaultConnection);
            DbHelper.ConnectionString.Add("daylily", secret.ConnectionStrings.MyConnection);

            OsuApiConfig.ApiKey = secret.OsuSettings.ApiKey;
            OsuApiConfig.UserName = secret.OsuSettings.UserName;
            OsuApiConfig.Password = secret.OsuSettings.Password;

            Signature.AppId = secret.CosSettings.AppId;
            Signature.SecretId = secret.CosSettings.SecretId;
            Signature.SecretKey = secret.CosSettings.SecretKey;
            Signature.BucketName = secret.CosSettings.BucketName;

            CoolQHttpApiClient.ApiUrl = secret.BotSettings.PostUrl;
            CoolQCode.CqPath = secret.BotSettings.CqDir;
            DaylilyCore.Current.CommandFlag = secret.BotSettings.CommandFlag;

            TuLingSecret.ApiKey = secret.TuLingSettings.ApiKey;
        }
    }
}
