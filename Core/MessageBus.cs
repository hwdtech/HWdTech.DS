using System.Threading;

using HWdTech.Common;
using HWdTech.DS.Internals;

namespace HWdTech.DS.Core
{
    public class MessageBus
    {
        static IRouter router;

        public static void Send(IMessage message)
        {
            if (null == router)
            {
                router = Singleton<DIContainer>.Instance.Resolve<IRouter>();
            }

            ThreadPool.QueueUserWorkItem(HandleMessage, message);

        }

        static void HandleMessage(object o)
        {
            IMessage message = (IMessage) o;

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
