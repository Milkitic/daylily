using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            services.AddMvc();
            DbHelper.ConnectionString.Add("cabbage", Configuration.GetConnectionString("DefaultConnection"));
            DbHelper.ConnectionString.Add("daylily", Configuration.GetConnectionString("MyConnection"));
            HttpApi.ApiUrl = Configuration.GetConnectionString("PostUrl");
            //Interface.DaylilyAssist.AssistApi.ApiUrl = Configuration.GetConnectionString("AssistUrl");
            Signature.appId = int.Parse(Configuration.GetConnectionString("appId"));
            Signature.secretId = Configuration.GetConnectionString("secretId");
            Signature.secretKey = Configuration.GetConnectionString("secretKey");
            Signature.bucketName = Configuration.GetConnectionString("bucketName");
            CQCode.CQRoot = Configuration.GetConnectionString("CQDir");
            OsuApi.ApiKey = Configuration.GetConnectionString("ApiKey");
            MessageHandler.COMMAND_FLAG = Configuration.GetConnectionString("commandFlag");
            RunService();
        }

        private void RunService()
        {
            Logger.PrimaryLine("读取服务中...");
            foreach (var item in Mapper.ServicePlugins)
            {
                string fullName = item.Value;
                string className = item.Key.Replace(".dll", "");
                MethodInfo mi;
                object appClass;
                Type type;
                System.IO.FileInfo fi = null;

                try
                {
                    Logger.InfoLine("读取" + item.Key + "中...");
                    fi = new System.IO.FileInfo(fullName);
                    Assembly assemblyTmp = Assembly.LoadFrom(fullName);
                    type = assemblyTmp.GetType(className);
                    appClass = assemblyTmp.CreateInstance(className);
                }
                catch (Exception ex)
                {
                    Logger.DangerLine(item.Key + " 出现了问题。");
                    if (ex.InnerException != null)
                        throw new Exception("\n\"" + className + "\" caused an exception: \n" +
                            fi.Name + ": " + ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace);
                    else
                        throw new Exception("\n\"" + className + "\" caused an exception: \n" +
                            fi.Name + ": " + ex.Message + "\n\n" + ex.StackTrace);
                }

                object[] invokeArgs = { };

                CommonMessageResponse reply = null;
                try
                {
                    mi = type.GetMethod("Run");
                    reply = (CommonMessageResponse)mi.Invoke(appClass, invokeArgs);
                }
                catch (Exception ex)
                {
                    Logger.DangerLine(item.Key + " 出现了问题。");
                    if (ex.InnerException != null)
                        throw new Exception("\n/\"" + className + "\" caused an exception: \n" +
                            type.Name + ": " + ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace);
                    else
                        throw new Exception("\n/\"" + className + "\" caused an exception: \n" +
                            type.Name + ": " + ex.Message + "\n\n" + ex.StackTrace);
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
