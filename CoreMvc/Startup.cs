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
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
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
            #region ���ÿ�������
            var urls = Configuration.GetSection("Cors:AllowOrigins").Value.Split(',');
            services.AddCors(option => option.AddPolicy("cors",
                policy =>
                policy
                      .WithOrigins(urls)//�������������վ��
                      .AllowAnyHeader()//������������ͷ
                      .AllowAnyMethod()//������������
                      .AllowCredentials()//����Я��cookie��Ϣ�����Ҫ���õ���
                      ));
            #endregion

            services.AddControllersWithViews();
            services.AddDbContext<CoreDemoDBContext>(o =>
            {
                o.UseSqlServer(Configuration.GetSection("ConnectionStrings:Default").Value);
            });

            //services.AddTransient<>
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            //ע��ִ�
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IEFRepository<>), typeof(EFRepository<>));

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

            //ִ�п���������м����corsΪ�Զ���Ŀ����������
            app.UseCors("cors");

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMiddleware<Middleware>();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }

}
