using System;
using System.Collections.Concurrent;

using HWdTech.DS.Core;

namespace HWdTech.DS.Internals.Implementation
{
    public class RouterImpl: IRouter
    {
        public RouterImpl()
        {
            routerImpl = new ConcurrentDictionary<string, Action<IMessage>>();
        }

        public void Send(IMessage message)
        {
            Action<IMessage> handler;
            if (routerImpl.TryGetValue(message.Target, out handler))
            {
                handler(message);
            }
            else
            {
                //ToDo: адресат неизвестен - надо разрешить с помощью внешнего сервиса.
            }
        }

        public IRouter RegisterOrReplace(string channel, Action<IMessage> handler)
        {
            routerImpl.AddOrUpdate(channel, handler, (key, oldValue) => { return handler; });
            return this;
        }
        ConcurrentDictionary<string, Action<IMessage>> routerImpl;
    }
}
