using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Moq;
using NUnit.Framework;

using HWdTech.Common;
using HWdTech.DS.Core;

using HWdTech.DS.Internals.Implementation;

namespace CoreTests
{
    [TestFixture]
    public class MessageBusTests
    {
        MockRepository repository = new MockRepository(MockBehavior.Strict);

        class PingActor : Actor
        {
            int gotMessages;
            IMessage pongMessage;
            ManualResetEvent signal;

            const int numberOfResponsesShoulgGet = 5;

            public PingActor(IMessage pongMessage, ManualResetEvent signal)
                : base("ping")
            {
                this.signal = signal;
                this.pongMessage = pongMessage;
                
                MessageBus.Send(pongMessage);
            }

            public override void Handle(IMessage message)
            {
                ++gotMessages;
                if (numberOfResponsesShoulgGet > gotMessages)
                {
                    MessageBus.Send(pongMessage);
                }
                else
                {
                    signal.Set();
                }
            }

            public void Verify()
            {
                Assert.AreEqual(numberOfResponsesShoulgGet, gotMessages);
            }
        }

        class PongActor : Actor
        {
            IMessage pingMessage;

            public PongActor(IMessage pingMessage)
                : base("pong")
            {
                this.pingMessage = pingMessage;
            }

            public override void Handle(IMessage message)
            {
                MessageBus.Send(pingMessage);
            }
        }

        [Test]
        public void PingPongTest()
        {
            ThreadPoolImpl threadPool = new ThreadPoolImpl();
            RouterImpl router = new RouterImpl();

            Mock<IMessage> pingMessage = repository.Create<IMessage>();
            pingMessage.SetupGet(m => m.Target).Returns("ping");

            Actor pongActor = new PongActor(pingMessage.Object);

            Mock<IMessage> pongMessage = repository.Create<IMessage>();
            pongMessage.SetupGet(m => m.Target).Returns("pong");

            ManualResetEvent waitSignal = new ManualResetEvent(false);
            PingActor pingActor = new PingActor(pongMessage.Object, waitSignal);

            Assert.True(waitSignal.WaitOne(1000));
            pingActor.Verify();
            
        }
    }
}
