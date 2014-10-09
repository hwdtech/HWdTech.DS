using Autofac;

namespace HWdTech.DS.Internals.Implementation
{
    class RouterRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            IRouter router = new RouterImpl();

            builder.RegisterInstance(router).As<IRouter>();
        }
    }
}
