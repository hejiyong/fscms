using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using FsCms.Service.Ioc;

namespace FsCms.Web.Startups
{
    public class ControllerPack : IDependencyRegistrar
    {
        /// <summary>
        /// 
        /// </summary>
        public int Order
        {
            get
            {
                return 3;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="listType"></param>
        public void Register(ContainerBuilder builder, List<Type> listType)
        {
            Console.WriteLine("Controller:控制器注册开始");
            builder.RegisterType(typeof(Support.AutofacModule.intercept.AOPIntercept));
            //注册Controller,实现属性注入
            var registrarType = typeof(Controller);
            var arrControllerType = listType.Where(t => t.BaseType == registrarType || t.BaseType == typeof(AdminBaseController)).ToArray();

            foreach (var item in arrControllerType)
            {
                Console.WriteLine($"Controller.List:{item.FullName}");
            }
            builder.RegisterTypes(arrControllerType)
                .AsSelf()
                .OnRegistered(e => Console.WriteLine("Controller：OnRegistered在注册的时候调用!"))
                .OnPreparing(e => Console.WriteLine("Controller：OnPreparing在准备创建的时候调用!"))
                .OnActivating(e => Console.WriteLine("Controller：OnActivating在创建之前调用!"))
                .OnActivated(e => Console.WriteLine("Controller：OnActivated创建之后调用!"))
                .OnRelease(e => Console.WriteLine("Controller：OnRelease在释放占用的资源之前调用!"))
                .EnableClassInterceptors()
                .InstancePerLifetimeScope()
                .PropertiesAutowired();


            //var DALFactoryType = typeof(DALFactory);
            //var DALFactoryTypes = listType.Where(t => t == DALFactoryType).ToArray();
            //foreach (var item in DALFactoryTypes)
            //{
            //    Console.WriteLine($"Controller.Factory:{item.FullName}");
            //}
            //builder.RegisterTypes(DALFactoryTypes)
            //  .AsSelf()
            //  //.OnRegistered(e => Console.WriteLine("Controller：OnRegistered在注册的时候调用!"))
            //  //.OnPreparing(e => Console.WriteLine("Controller：OnPreparing在准备创建的时候调用!"))
            //  //.OnActivating(e => Console.WriteLine("Controller：OnActivating在创建之前调用!"))
            //  //.OnActivated(e => Console.WriteLine("Controller：OnActivated创建之后调用!"))
            //  //.OnRelease(e => Console.WriteLine("Controller：OnRelease在释放占用的资源之前调用!"))
            //  .EnableClassInterceptors()
            //  .InstancePerLifetimeScope()
            //  .PropertiesAutowired();

            Console.WriteLine("Controller:控制器注册成功");
        }
    }
}
