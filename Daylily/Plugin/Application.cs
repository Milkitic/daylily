using Daylily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Daylily.Database;
using Daylily.Interface.CQHttp;
using Daylily.Assist;
using Daylily.Interface;

namespace Daylily.Plugin
{
    public abstract class Application
    {
        protected PermissionLevel CurrentLevel { get; set; }

        public Random rnd = new Random();

        public abstract string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt, long messageId);

        public Application()
        {
            string jsonString = File.ReadAllText("appsettings.json");
            var obj = JsonConvert.DeserializeObject<JsonSettings>(jsonString);
            try
            {
                DbHelper.ConnectionString.Add("cabbage", obj.ConnectionStrings.DefaultConnection);
                DbHelper.ConnectionString.Add("daylily", obj.ConnectionStrings.MyConnection);
            }
            catch { }
            HttpApi.ApiUrl = obj.ConnectionStrings.PostUrl;
            CQCode.CQRoot = obj.ConnectionStrings.CQDir;
            OsuApi.ApiKey = obj.ConnectionStrings.ApiKey;
        }
    }
}
