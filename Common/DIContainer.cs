using System;
using System.Reflection;

using Autofac;
using Autofac.Core;

namespace HWdTech.Common
{
    public class DIContainer
    {
        IContainer container;

        public DIContainer(IContainer container)
        {
            this.container = container;
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public static void Init()
        {
            ContainerBuilder builder = new ContainerBuilder();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterAssemblyModules(assemblies);

            Singleton<DIContainer>.Instance = new DIContainer(builder.Build());
        }
    }
}
