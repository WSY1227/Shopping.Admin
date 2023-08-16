using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace WebApi.Config;

public class AutofacModuleRegister: Module
{
    /// <summary>
    /// 重写Autofac管道Load方法，在这里注册注入
    /// </summary>
    /// <param name="builder"></param>
    protected override void Load(ContainerBuilder builder)
    {
        var interfaceAssembly = Assembly.Load("Interface");
        var serviceAssembly = Assembly.Load("Service");
/**         builder.RegisterAssemblyTypes(interfaceAssembly, serviceAssembly)
            .Where(t => t.Name.EndsWith("Svc"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
            此段代码是注入所有以Svc结尾的类,并且.InstancePerLifetimeScope()来指定每个作用域（请求）创建一个新的实例
            **/
        builder.RegisterAssemblyTypes(interfaceAssembly, serviceAssembly).AsImplementedInterfaces();
    }
}