using System;
using System.Threading.Tasks;
using Daylily.Bot.Function;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.Socket;
using Daylily.CoolQ;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.Cos;
using Daylily.Osu.Database;
using Daylily.Osu.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Daylily.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 读设置
            DbHelper.ConnectionString.Add("cabbage", Configuration.GetConnectionString("DefaultConnection"));
            DbHelper.ConnectionString.Add("daylily", Configuration.GetConnectionString("MyConnection"));

            OsuApiKey.ApiKey = (string)Configuration.GetSection("OsuSettings").GetValue(typeof(string), "ApiKey");

            Signature.AppId = (int)Configuration.GetSection("CosSettings").GetValue(typeof(int), "appId");
            Signature.SecretId = (string)Configuration.GetSection("CosSettings").GetValue(typeof(string), "secretId");
            Signature.SecretKey = (string)Configuration.GetSection("CosSettings").GetValue(typeof(string), "secretKey");
            Signature.BucketName = (string)Configuration.GetSection("CosSettings").GetValue(typeof(string), "bucketName");

            CqApi.ApiUrl = (string)Configuration.GetSection("BotSettings").GetValue(typeof(string), "PostUrl");
            CqCode.CqRoot = (string)Configuration.GetSection("BotSettings").GetValue(typeof(string), "CQDir");
            MessageHandler.CommandFlag = (string)Configuration.GetSection("BotSettings").GetValue(typeof(string), "commandFlag");

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);

            async Task WebSocket(HttpContext context, Func<Task> next)
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        SocketLogger.WebSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await SocketLogger.Response();
                    }
                    else
                        context.Response.StatusCode = 400;
                }
                else
                    await next();
            }

            app.Use(WebSocket);

            Logger.Raw("Websocket控制台已启动。");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            Bot.Console.Startup.RunConsole();
            if (MessageHandler.PrivateDisabledList == null)
                MessageHandler.PrivateDisabledList =
                    new System.Collections.Concurrent.ConcurrentDictionary<long, System.Collections.Generic.List<string>>();
            if (MessageHandler.DiscussDisabledList == null)
                MessageHandler.DiscussDisabledList =
                    new System.Collections.Concurrent.ConcurrentDictionary<long, System.Collections.Generic.List<string>>();
            if (MessageHandler.GroupDisabledList == null)
                MessageHandler.GroupDisabledList =
                    new System.Collections.Concurrent.ConcurrentDictionary<long, System.Collections.Generic.List<string>>();



            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
