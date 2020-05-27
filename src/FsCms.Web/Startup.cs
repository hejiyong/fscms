
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using FsCms.Web.Startups;
using FsCms.Service.Ioc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel.Design;
using Autofac;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Linq;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.Hosting;

namespace FsCms.Web
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
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/login");
                    options.AccessDeniedPath = new PathString("/denied");
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //Session服务
            services.AddSession();

            services.AddControllersWithViews();

            //添加跨域访问
            services.AddCors(options => options.AddPolicy("AllowAnyOrigin",
                builder => builder.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowCredentials()));

            //hangfire
            //log
            //redis
            //映射
            //config
            //ioc

            services.AddControllers(options => { })
                .AddNewtonsoftJson(options =>
            {
                //将json返回字段修改驼峰
                options.SerializerSettings.ContractResolver = new DefaultContractResolver() { };
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //options.SerializerSettings.NullValueHandling
                //忽略循环引用
                //optionJsonSerializerOptions.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //设置时间格式
                //options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            //添加对AutoMapper的支持
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //初始化数据
            new SeedData().Initialize();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //显示Autofac注入
            //IocManager.Instance.Initialize(builder);
            //Assembly assembly = Assembly.Load("FsCms.Service");
            //builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

            //builder.RegisterInstance(Instance).As<IIocManager>().SingleInstance();
            //所有程序集 和程序集下类型
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");//排除所有的系统程序集、Nuget下载包
            var listAllType = new List<Type>();
            foreach (var lib in libs)
            {
                try
                {
                    var _assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    listAllType.AddRange(_assembly.GetTypes().Where(type => type != null));
                }
                catch { }
            }
            //找到所有外部IDependencyRegistrar实现，调用注册
            var registrarType = typeof(IDependencyRegistrar);
            var arrRegistrarType = listAllType.Where(t => registrarType.IsAssignableFrom(t) && t != registrarType).ToArray();
            var listRegistrarInstances = new List<IDependencyRegistrar>();
            foreach (var drType in arrRegistrarType)
            {
                listRegistrarInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            }
            //排序
            listRegistrarInstances = listRegistrarInstances.OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in listRegistrarInstances)
            {
                dependencyRegistrar.Register(builder, listAllType);
            }

            //注册ITransientDependency实现类
            var dependencyType = typeof(ITransientDependency);
            var arrDependencyType = listAllType.Where(t => dependencyType.IsAssignableFrom(t) && t != dependencyType).ToArray();
            builder.RegisterTypes(arrDependencyType)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .PropertiesAutowired().EnableInterfaceInterceptors();

            foreach (Type type in arrDependencyType)
            {
                if (type.IsClass && !type.IsAbstract && !type.BaseType.IsInterface && type.BaseType != typeof(object))
                {
                    builder.RegisterType(type).As(type.BaseType)
                        .InstancePerLifetimeScope()
                        .PropertiesAutowired();
                }
            }


            //注册ISingletonDependency实现类
            var singletonDependencyType = typeof(ISingletonDependency);
            var arrSingletonDependencyType = listAllType.Where(t => singletonDependencyType.IsAssignableFrom(t) && t != singletonDependencyType).ToArray();
            builder.RegisterTypes(arrSingletonDependencyType)
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired();

            foreach (Type type in arrSingletonDependencyType)
            {
                if (type.IsClass && !type.IsAbstract && !type.BaseType.IsInterface && type.BaseType != typeof(object))
                {
                    builder.RegisterType(type).As(type.BaseType)
                        .SingleInstance()
                        .PropertiesAutowired();
                }
            }

            //builder.Populate(services);
            //_container = builder.Build();
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseSession();

            app.UseRouting();

            //app.Use((context, next) =>
            //{
            //    var route = context.GetRouteData();
            //    var endpoint = context.GetEndpoint();//拿到终结点                
            //    var routeData = context.Request.RouteValues;//拿到路由数据
            //    Console.WriteLine("=======");

            //    Console.WriteLine("route：" + route.ToJson());
            //    Console.WriteLine("endpoint：" + endpoint?.DisplayName);
            //    Console.WriteLine("routeData：" + routeData.ToJson());                             //做些牛B的事
            //    return next();
            //});
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
            //endpoints.MapAreaControllerRoute(
            //    name: "MyAreaAdmin",
            //    areaName: "Admin",
            //    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
              name: "MyArea",
              pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //endpoints.MapControllerRoute(
            //    name: "area",
            //    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //endpoints.MapAreaControllerRoute(
            //    name: "areas",
            //    areaName: "areas",
            //    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}",
            //    defaults: new { Area = "admin", Controller = "Home", Action = "Index" });

            endpoints.MapControllerRoute(
               name: "default",
               pattern: "{controller=Home}/{action=Index}/{id?}");

            //endpoints.MapBlazorPages();
        });
        }
    }
}
