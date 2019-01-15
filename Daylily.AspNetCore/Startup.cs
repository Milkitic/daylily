using Daylily.Bot;
using Daylily.Bot.Dispatcher;
using Daylily.Bot.Frontend;
using Daylily.Common.Logging;
using Daylily.CoolQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Daylily.AspNetCore
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var metadata = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application;
            services.AddDaylily(
                new StartupConfig(
                    new CoolQDispatcher().Config(obj => { }),
                    new IFrontend[]
                    {
                        new CoolQFrontend().Config(obj =>{})
                    },
                    new StartupConfig.Metadata
                    {
                        ApplicationName = metadata.ApplicationName,
                        FrameworkName = metadata.RuntimeFramework
                    }
                ), Load.LoadSecret
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DaylilyCore daylily)
        {
            foreach (var frontend in daylily.Frontends)
            {
                if (frontend is CoolQFrontend cqFrontend && daylily.Dispatcher is CoolQDispatcher cqDispatcher)
                {
                    //cqFrontend.PrivateMessageReceived += (sender, e) => { };
                    //cqFrontend.DiscussMessageReceived+= (sender, e) => { };
                    //cqFrontend.GroupMessageReceived += (sender, e) => { };
                    cqFrontend.Requested += (sender, e) =>
                    {
                        daylily.EventDispatcher?.Event_Received(sender, new EventEventArgs(e.ParsedObject));
                    };
                    cqFrontend.Noticed += (sender, e) =>
                    {
                        daylily.EventDispatcher?.Event_Received(sender, new EventEventArgs(e.ParsedObject));
                    };
                    //cqFrontend.EventReceived += (sender, e) =>
                    //{
                    //    daylily.EventDispatcher?.Event_Received(sender, e);
                    //};
                }

                frontend.MessageReceived += (sender, e) =>
                {
                    daylily.MessageDispatcher?.Message_Received(sender, new MessageEventArgs(e.ParsedObject));
                };
                frontend.ErrorOccured += (sender, e) => { };
            }

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
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            //app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
