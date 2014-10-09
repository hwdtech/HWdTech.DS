using System;
using System.Reflection;

using Autofac;
using NUnit.Framework;

namespace HWdTech.DS.Internals.Implementation.Tests
{
    [TestFixture(Category= "Internals")]
    public class IoCRegistrationTests
    {
        [Test]
        public void ThreadPoolImplShouldBeRegisteredAsInterfaceIThreadPoolImplementationInAutofac()
        {
            ContainerBuilder builder = new ContainerBuilder();

            Assembly assembly = Assembly.GetAssembly(typeof(ThreadPoolImpl));

            builder.RegisterAssemblyModules(assembly);

            var container = builder.Build();

            container.Resolve<IThreadPool>();
        }
    }
}
