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
        
        IMessage message;

        [SetUp]
        public void SetUp()
        {
            ThreadPool.SetMaxThreads(16, 8);
            ThreadPool.SetMinThreads(16, 8);

            Mock<IMessage> messageMock = repository.Create<IMessage>();
            message = messageMock.Object;
        }


        [Test]
        public void AnActorShouldHandleAMessage()
        {
            Mock<Actor> actor = repository.Create<Actor>();

            actor.Setup(a => a.Handle(message)).Verifiable();

            actor.Object.Receive(message);

            actor.VerifyAll();
        }

        class MockActorForCheckBecomeMethod : Actor
        {
            bool calledFirstBehaviour = false;
            int calledSecondBehaviour = 0;

            public override void Handle(IMessage message)
            {
                calledFirstBehaviour = true;
                Become(SecondHandle);
            }

            public void SecondHandle(IMessage message)
            {
                ++calledSecondBehaviour;
            }

            public void VerifyAll()
            {
                Assert.True(calledFirstBehaviour);
                Assert.AreEqual(2, calledSecondBehaviour);
            }
        }

        [Test]
        public void AnActorShouldChangeItsBehaviourUsingBecome()
        {
            MockActorForCheckBecomeMethod actor = new MockActorForCheckBecomeMethod();

            actor.Receive(message);
            actor.Receive(message);
            actor.Receive(message);

            actor.VerifyAll();
        }

        ManualResetEvent waitUntilMessagesAreHandled = new ManualResetEvent(false);

        const int total = 100000;

        class MockActor: Actor
        {
            ManualResetEvent signal;

            public MockActor(ManualResetEvent signal)
            {
                this.signal = signal;
            }

            int counter = 0;

            public override void Handle(IMessage m)
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
            StateLessActor actor = new StateLessActor(waitUntilMessagesAreHandled);

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
            Actor actor = new MockActor(waitUntilMessagesAreHandled);

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
