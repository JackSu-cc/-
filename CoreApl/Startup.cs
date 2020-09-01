using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace CoreApl
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //���controller����(webapi)
            services.AddControllers();
            //������ݿ�����
            services.AddDbContext<CoreDemoDBContext>(o =>
            {
                o.UseSqlServer(Configuration.GetSection("ConnectionStrings:Default").Value);
            });

            //ע�뻺��
            services.AddSingleton<IMemoryCache, MemoryCache>();

            //services.AddTransient<>
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            //ע��ִ�
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IEFRepository<>), typeof(EFRepository<>));

            #region ����Swagger���� 
            //����Swagger
            services.AddSwaggerGen(c =>
            {
                var version = "v1";
                c.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = $"{Configuration.GetSection("BasicSettings:apiName").Value} CoreAPI�ӿ��ĵ�����dotnetcore 3.1",//�༭����
                    Version = version,//�汾��
                    Description = $"{Configuration.GetSection("BasicSettings:apiName").Value} HTTP API V1",//�༭����
                    Contact = new OpenApiContact { Name = $"{ Configuration.GetSection("BasicSettings:apiName").Value }-���Ҹ�����Ա���ʼ�", Email = "929013002@qq.com" },//�༭��ϵ��ʽ
                    License = new OpenApiLicense { Name = Configuration.GetSection("BasicSettings:apiName").Value }//�༭���֤
                });
                c.OrderActionsBy(o => o.RelativePath);//����˳��
                                                      
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CoreApl.xml");// ���ýӿ��ĵ��ļ�·��
                c.IncludeXmlComments(xmlPath, true); // �ѽӿ��ĵ���·�����ý�ȥ���ڶ���������ʾ�����Ƿ���������Controller��ע������
            });
            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            #region ���Swagger�м��

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });
            #endregion
        }
    }
}
