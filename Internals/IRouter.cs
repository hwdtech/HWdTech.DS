using System;

using HWdTech.DS.Core;

namespace HWdTech.DS.Internals
{
    public interface IRouter
    {
        void Send(IMessage message);
        IRouter RegisterOrReplace(string channel, Action<IMessage> handler);
    }
}
