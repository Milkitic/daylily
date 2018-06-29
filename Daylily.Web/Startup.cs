using System;
using System.Reflection;
using Daylily.Common.Assist;
using Daylily.Common.Database;
using Daylily.Common.Function;
using Daylily.Common.Interface;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models;
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
            // 读设置
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
            services.AddMvc();
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
