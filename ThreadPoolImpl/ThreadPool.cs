using System;

namespace HWdTech.DS.Internals.Implementation
{
    public class ThreadPoolImpl : IThreadPool
    {
        public void StartTask(Action<object> action, object args = null)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(action), args);
        }
    }

}
