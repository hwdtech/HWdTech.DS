using System.Collections.Concurrent;
using System.Threading;

namespace HWdTech.DS.Core
{
    public abstract class Actor
    {
        int requests = 0;
        int isHandler = 0;
        ConcurrentQueue<IMessage> queue = new ConcurrentQueue<IMessage>();

        public void Receive(IMessage message)
        {
            bool taskNotQueued = true;
            Interlocked.Increment(ref requests);
            if (0 == Interlocked.Exchange(ref isHandler, 1))
            {
                Handle(message);

                ProcessQueueTillCanRead();

                Interlocked.Exchange(ref isHandler, 0);

                if (queue.Count > 0)
                {
                    taskNotQueued = false;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessQueue));
                }
            }
            else
            {
                queue.Enqueue(message);
            }
            if (0 == Interlocked.Decrement(ref requests))
            {
                if (taskNotQueued)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessQueue));
                }
            }
        }

        private void ProcessQueueTillCanRead()
        {
            IMessage m;
            while (queue.TryDequeue(out m))
            {
                Handle(m);
            }
        }

        internal void ProcessQueue(object o)
        {
            if (0 == Interlocked.Exchange(ref isHandler, 1))
            {
                ProcessQueueTillCanRead();

                Interlocked.Exchange(ref isHandler, 0);

                if (queue.Count > 0)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessQueue));
                }
            }
        }


        protected abstract void Handle(IMessage message);
    }
}
