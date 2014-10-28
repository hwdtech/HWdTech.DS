using System;
using System.Reflection;

using Autofac;
using Autofac.Core;

namespace HWdTech.Common
{
    public class DIContainer
    {
        IContainer container;

        public DIContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyModules(assemblies);
            this.container = builder.Build();

            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoadEventHandler;
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public static void Init()
        {
            Singleton<DIContainer>.Instance = new DIContainer();            
        }

        private void AssemblyLoadEventHandler(object sender, AssemblyLoadEventArgs args)
        {
            LoadAssembly(args.LoadedAssembly);
        }

        private void LoadAssembly(Assembly assembly)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(assembly);
            builder.Update(container);
        }
    }
}
