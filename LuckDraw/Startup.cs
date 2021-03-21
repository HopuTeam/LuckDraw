using LuckDraw.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LuckDraw
{
    public class Startup
    {
        public IConfiguration Configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                //options.Filters.Add(typeof(ErrorAttribute));
                options.EnableEndpointRouting = false;
            });
            services.AddDbContext<CoreEntities>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("EFDbConnection"));
            });
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //开发环境直接展示错误堆栈的页面 500 only
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //正式环境自定义错误页
                app.UseExceptionHandler("/Views/Error/Index.cshtml");
                app.UseHsts();
            }
            app.UseRouting();
            app.UseSession();
            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/Error/{0}");//错误页跳转
            app.UseMvc(options =>
            {
                options.MapRoute("Default", "{Controller=Sign}/{Action=Index}");
            });
        }
    }
}