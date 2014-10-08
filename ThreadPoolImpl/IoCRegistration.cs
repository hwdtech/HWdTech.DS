using Autofac;

namespace HWdTech.DS.Internals.Implementation
{
    class ThreadPoolRegistrationModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            IThreadPool pool = new ThreadPoolImpl();

            builder.RegisterInstance(pool).As<IThreadPool>();
        }
    }
}
