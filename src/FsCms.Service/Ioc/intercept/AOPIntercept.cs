using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Support.AutofacModule.intercept
{
    public class AOPIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("你正在调用方法 \"{0}\"  参数是 {1}... ",
               invocation.Method.Name,
               string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));

            invocation.Proceed();
            if (invocation.ReturnValue != null && invocation.ReturnValue is string)
            {
                //在返回接口上拼上LogInterceptor
                invocation.ReturnValue += " LogInterceptor";
            }
            Console.WriteLine("方法执行完毕，返回结果：{0}", invocation.ReturnValue);

            Console.WriteLine("开始记录日志....");
        }
    }
}
