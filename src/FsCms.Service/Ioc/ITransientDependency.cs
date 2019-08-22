namespace FsCms.Service.Ioc
{

    /// <summary>
    /// 所有接口的依赖接口，每次创建新实例
    /// </summary>
    /// <remarks>
    /// 用于Autofac自动注册时，查找所有依赖该接口的实现。
    /// 实现自动注册功能
    /// </remarks>
    public interface ITransientDependency
    {
    }
}