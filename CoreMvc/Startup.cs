using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.IService.IUserService;
using Application.Service.UserService;
using Common.BaseInterfaces.IBaseRepository;
using Common.BaseInterfaces.IBaseRepository.IRepository;
using Domain.IRepository;
using Infrastruct.Context;
using Infrastruct.Repository.BaseRepository;
using Infrastruct.Repository.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreMvc
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
            #region 配置跨域请求
            var urls = Configuration.GetSection("Cors:AllowOrigins").Value.Split(',');
            services.AddCors(option => option.AddPolicy("cors",
                policy =>
                policy
                      .WithOrigins(urls)//配置允许请求的站点
                      .AllowAnyHeader()//允许所有请求头
                      .AllowAnyMethod()//允许请求类型
                      .AllowCredentials()//允许携带cookie信息（这个要禁用掉）
                      ));
            #endregion

            services.AddControllersWithViews();
            //services.AddDbContext<CoreDemoDBContext>(o =>
            //{
            //    o.UseSqlServer(Configuration.GetSection("ConnectionStrings:Default").Value);
            //});
            //注入缓存
            services.AddSingleton<IMemoryCache, MemoryCache>();

            //services.AddTransient<>
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //注入仓储
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped(typeof(IEFRepository<>), typeof(EFRepository<>));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //执行跨域请求的中间件，cors为自定义的跨域请求策略
            app.UseCors("cors");

            // app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthorization();

            #region 自定义中间件，学习
            //自定义中间件>>方式1
            //app.UseMiddleware<Middleware>();

            //自定义中间件>>方式2
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("<p>假如这个是最后一个啊</p>");
            //    await next.Invoke();//不加这个，不会执行下一个中间件

            //});
            //自定义中间件>>方式3
            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("<p>最后一个啊</p>");
            //});

            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                     name:"default2",
                     areaName:"System",
                      pattern: "System/{controller=Home}/{action=Index}"
                    );
            });


        }

    }

}
