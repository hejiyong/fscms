using Autofac;
using FsCms.Service.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FsCms.Web.Startups
{
    public class EntityPack : IDependencyRegistrar
    {
        /// <summary>
        /// 
        /// </summary>
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, List<Type> listType)
        {
            Console.WriteLine("service:服务注册开始");
            var registrarType = typeof(ISingletonDependency);
            var arrControllerType = listType.Where(t => registrarType.IsAssignableFrom(t) && t != registrarType).ToArray();

            foreach (var item in arrControllerType)
            {
                Console.WriteLine($"service:{item.FullName}");
            }

            //构造函数注册
            builder.RegisterTypes(listType.Where(t => registrarType.IsAssignableFrom(t) && t != registrarType).ToArray())
                .AsSelf()
              .InstancePerLifetimeScope()
              .PropertiesAutowired();

            ////属性注册
            //var attributeRegisterType = typeof(IRegisterByAttribute);
            //var arrDALType = listType.Where(t => attributeRegisterType.IsAssignableFrom(t) && t != attributeRegisterType).ToArray();

            //foreach (var item in arrDALType)
            //{
            //    Console.WriteLine($"service:Attribute:{item.FullName}");
            //}

            //builder.RegisterTypes(arrDALType)
            //   .AsSelf()
            //  .OnRegistered(e => Console.WriteLine("service:OnRegistered在注册的时候调用!" + listType.Where(t => attributeRegisterType.IsAssignableFrom(t) && t != attributeRegisterType).ToArray().Count()))
            //  .OnPreparing(e => Console.WriteLine("service:OnPreparing在准备创建的时候调用!"))
            //  .OnActivating(e => Console.WriteLine("service:OnActivating在创建之前调用!"))
            //  .OnActivated(e => Console.WriteLine("service:OnActivated创建之后调用!"))
            //  .OnRelease(e => Console.WriteLine("service:OnRelease在释放占用的资源之前调用!"))
            //  .InstancePerLifetimeScope()
            //  .PropertiesAutowired();
            Console.WriteLine("service:服务注册结束");
        }
    }

}
