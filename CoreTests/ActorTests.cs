using System;
using System.Threading;

using Moq;
using NUnit.Framework;

namespace HWdTech.DS.Core.Tests
{
    [TestFixture]
    public class ActorTests
    {
        MockRepository repository = new MockRepository(MockBehavior.Strict);

        ManualResetEvent waitUntilMessagesAreHandled = new ManualResetEvent(false);

        const int total = 1000000;

        class MockActor : Actor
        {
            ManualResetEvent signal;

            public MockActor(ManualResetEvent signal)
            {
                this.signal = signal;
            }

            int counter = 0;

            protected override void Handle(IMessage m)
            {
                if (Interlocked.Increment(ref counter) == total)
                {
                    signal.Set();
                }
            }
        }

        class StateLessActor
        {
            ManualResetEvent signal;

            public StateLessActor(ManualResetEvent signal)
            {
                this.signal = signal;
            }

            int counter = 0;

            public void Receive(IMessage m)
            {
                if (Interlocked.Increment(ref counter) == total)
                {
                    signal.Set();
                }
            }
        }

        [Test]
        public void StateLessActorsLoadTest()
        {
            ThreadPool.SetMaxThreads(16, 8);
            ThreadPool.SetMinThreads(16, 8);

            StateLessActor actor = new StateLessActor(waitUntilMessagesAreHandled);

            Mock<IMessage> messageMock = repository.Create<IMessage>();
            IMessage message = messageMock.Object;

            DateTime start = DateTime.Now;

            for (int i = 0; i < total; ++i)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (o) => { actor.Receive(message); }
                ));
            }

            Assert.True(waitUntilMessagesAreHandled.WaitOne(4000));
            DateTime end = DateTime.Now;

            Console.WriteLine("Execution time of test is {0}", end - start);
        }

        [Test]
        public void ActorsLoadTest()
        {
            ThreadPool.SetMaxThreads(16, 8);
            ThreadPool.SetMinThreads(16, 8);

            Actor actor = new MockActor(waitUntilMessagesAreHandled);

            Mock<IMessage> messageMock = repository.Create<IMessage>();
            IMessage message = messageMock.Object;

            DateTime start = DateTime.Now;

            for (int i = 0; i < total; ++i)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (o) => { actor.Receive(message); }
                ));
            }

            Assert.True(waitUntilMessagesAreHandled.WaitOne(4000));
            DateTime end = DateTime.Now;

            Console.WriteLine("Execution time of test is {0}", end - start);
        }

        [Test]
        public void ThreadPoolLoadTest()
        {
            ThreadPool.SetMaxThreads(16, 8);
            ThreadPool.SetMinThreads(16, 8);

            int counter = 0;

            DateTime start = DateTime.Now;

            for (int i = 0; i < total; ++i)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (o) => {
                        if (Interlocked.Increment(ref counter) == total)
                        {
                            waitUntilMessagesAreHandled.Set();
                        }
                    }
                ));
            }

            Assert.True(waitUntilMessagesAreHandled.WaitOne(4000));
            DateTime end = DateTime.Now;

            Console.WriteLine("Execution time of test is {0}", end - start);
        }

        [Test]
        public void SingleThreadTest()
        {
            int counter = 0;

            DateTime start = DateTime.Now;

            for (int i = 0; i < total; ++i)
            {
                ++counter;
                if (counter == total)
                {
                    break;
                }
            }

            DateTime end = DateTime.Now;

            Console.WriteLine("Execution time of test is {0}", end - start);
        }
    }
}
