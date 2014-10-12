using System.Threading;

using HWdTech.Common;
using HWdTech.DS.Internals;

namespace HWdTech.DS.Core
{
    public class MessageBus
    {
        static IRouter router;
        static IThreadPool threadPool;

        public static void Send(IMessage message)
        {
            if (null == threadPool)
            {
                threadPool = Singleton<DIContainer>.Instance.Resolve<IThreadPool>();
            }

            threadPool.StartTask(HandleMessage, message);
        }

        static void HandleMessage(object o)
        {
            IMessage message = (IMessage) o;

            if (null == router)
            {
                router = Singleton<DIContainer>.Instance.Resolve<IRouter>();
            }

            router.Send(message);
        }

        public static void Join(string address, Actor actor)
        {
            if (null == router)
            {
                router = Singleton<DIContainer>.Instance.Resolve<IRouter>();
            }

            router.RegisterOrReplace(address, actor.Receive);
        }
    }
}
