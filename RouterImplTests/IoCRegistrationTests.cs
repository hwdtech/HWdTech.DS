using System;
using System.Reflection;

using Autofac;
using NUnit.Framework;

namespace HWdTech.DS.Internals.Implementation.Tests
{
    [TestFixture(Category = "Internals")]
    public class IoCRegistrationRouterTests
    {
        [Test]
        public void RouterImplShouldBeRegisteredAsInterfaceRouterImplementationInAutofac()
        {
            ContainerBuilder builder = new ContainerBuilder();

            Assembly assembly = Assembly.GetAssembly(typeof(RouterImpl));

            builder.RegisterAssemblyModules(assembly);

            var container = builder.Build();

            container.Resolve<IRouter>();
        }
    }
}
