using System;
using System.Reflection;
using Daylily.Common.Assist;
using Daylily.Common.Database;
using Daylily.Common.Interface;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models;
using Daylily.Web.Function;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            DbHelper.ConnectionString.Add("cabbage", Configuration.GetConnectionString("DefaultConnection"));
            DbHelper.ConnectionString.Add("daylily", Configuration.GetConnectionString("MyConnection"));

            OsuApi.ApiKey = (string)Configuration.GetSection("OsuSettings").GetValue(typeof(string), "ApiKey");

            Signature.AppId = (int)Configuration.GetSection("CosSettings").GetValue(typeof(int), "appId");
            Signature.SecretId = (string)Configuration.GetSection("CosSettings").GetValue(typeof(string), "secretId");
            Signature.SecretKey = (string)Configuration.GetSection("CosSettings").GetValue(typeof(string), "secretKey");
            Signature.BucketName = (string)Configuration.GetSection("CosSettings").GetValue(typeof(string), "bucketName");

            CqApi.ApiUrl = (string)Configuration.GetSection("BotSettings").GetValue(typeof(string), "PostUrl");
            CqCode.CqRoot = (string)Configuration.GetSection("BotSettings").GetValue(typeof(string), "CQDir");
            MessageHandler.CommandFlag = (string)Configuration.GetSection("BotSettings").GetValue(typeof(string), "commandFlag");
            RunService();
            services.AddMvc();
        }

        private void RunService()
        {
            Logger.PrimaryLine("读取服务中...");
            foreach (var item in Mapper.ServicePlugins)
            {
                string fullName = item.Value;
                string className = item.Key.Replace(".dll", "");
                object appClass;
                Type type;

                try
                {
                    Logger.InfoLine("读取" + item.Key + "中...");
                    Assembly assemblyTmp = Assembly.LoadFrom(fullName);
                    type = assemblyTmp.GetType(className);
                    appClass = assemblyTmp.CreateInstance(className);
                }
                catch (Exception ex)
                {
                    Logger.DangerLine(item.Key + " 出现了问题。");
                    Logger.WriteException(ex);
                    return;
                }

                object[] invokeArgs = { };

                try
                {
                    MethodInfo mi = type.GetMethod("Run");
                    mi.Invoke(appClass, invokeArgs);
                }
                catch (Exception ex)
                {
                    Logger.DangerLine(item.Key + " 出现了问题。");
                    Logger.WriteException(ex);
                    return;
                }

                Logger.SuccessLine(item.Key + "已加载。");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseMvc(delegate (Microsoft.AspNetCore.Routing.IRouteBuilder routes)
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
