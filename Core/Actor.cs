using System;
using System.Collections.Concurrent;
using System.Threading;

namespace HWdTech.DS.Core
{
    public abstract class Actor
    {
        int requests = 0;
        int isHandler = 0;
        ConcurrentQueue<IMessage> queue = new ConcurrentQueue<IMessage>();

        Action<IMessage> handler;

        public Actor(string address)
        {
            this.handler = Handle;

            MessageBus.Join(address, this);
        }

        public Actor()
        {
            this.handler = Handle;
        }

        public void Become(Action<IMessage> handler)
        {
            this.handler = handler;
        }

        public abstract void Handle(IMessage message);

        public void Receive(IMessage message)
        {
            bool ReceiveMessageFromMailboxTaskNotSchedulled = true;

            AcceptRequest();

            try
            {

                if (CanHandleAMessage())
                {
                    try
                    {
                        HandleIncoming(message);

                        ReceiveMessagesFromMailbox();
                    }
                    finally
                    {
                        FinishMessagesHandling();
                        ReceiveMessageFromMailboxTaskNotSchedulled = !ScheduleReceviveMessagesFromMailboxToTheThreadPool();
                    }
                }
                else
                {
                    PutIncomingMessageToTheQueue(message);
                }
            }
            finally
            {
                if (IsLastRequest())
                {
                    if (ReceiveMessageFromMailboxTaskNotSchedulled)
                    {
                        ScheduleReceviveMessagesFromMailboxToTheThreadPool();
                    }
                }
            }
        }

        private void PutIncomingMessageToTheQueue(IMessage message)
        {
            if (message != null)
            {
                queue.Enqueue(message);
            }
        }

        private bool IsLastRequest()
        {
            return 0 == Interlocked.Decrement(ref requests);
        }

        private void AcceptRequest()
        {
            Interlocked.Increment(ref requests);
        }

        private void FinishMessagesHandling()
        {
            Interlocked.Exchange(ref isHandler, 0);
        }

        private bool CanHandleAMessage()
        {
            return 0 == Interlocked.Exchange(ref isHandler, 1);
        }

        private void ReceiveMessagesFromMailbox()
        {
            IMessage m;
            while (queue.TryDequeue(out m))
            {
                Handle(m);
            }
        }

        private bool ScheduleReceviveMessagesFromMailboxToTheThreadPool()
        {
            if (queue.Count > 0)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessQueue));
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void ProcessQueue(object o)
        {
            Receive(null);
        }


        private void HandleIncoming(IMessage message)
        {
            handler(message);
        }
    }
}
