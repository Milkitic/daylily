using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaylilyWeb.Assist;
using DaylilyWeb.Database;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DaylilyWeb
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
            Signature.appId = int.Parse(Configuration.GetConnectionString("appId"));
            Signature.secretId = Configuration.GetConnectionString("secretId");
            Signature.secretKey = Configuration.GetConnectionString("secretKey");
            Signature.bucketName = Configuration.GetConnectionString("bucketName");
            CQCode.CQRoot = Configuration.GetConnectionString("CQDir");
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
