using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Moq;
using NUnit.Framework;

using HWdTech.DS.Core;

namespace HWdTech.DS.Internals.Implementation.Tests
{
    [TestFixture(Category= "Internals")]
    public class RouteMessagesTests
    {
        MockRepository repository = new MockRepository(MockBehavior.Strict);
        Mock<IMessage> messageMock;

        string address = "targetAddress";

        RouterImpl router;

        [SetUp]
        public void SetupTest()
        {
            messageMock = repository.Create<IMessage>();
            messageMock.Setup<string>(m => m.Target).Returns(address);

            router = new RouterImpl();
        }

        [Test]
        public void RouterShouldProcessMessageIfHandlerIsRegistered()
        {
            bool wasCalled = false;
            router.RegisterOrReplace(address, (m) => {wasCalled = true;});

            router.Send(messageMock.Object);

            messageMock.VerifyAll();
            Assert.True(wasCalled);
        }

        [Test]
        public void RouterShouldProcessMessageIfHandlerIsNotRegistered()
        {
            router.Send(messageMock.Object);

            messageMock.VerifyAll();
        }
    }
}
