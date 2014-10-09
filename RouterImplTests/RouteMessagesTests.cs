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
        [Test]
        public void RouterShouldProcessMessageIfHandlerIsRegistered()
        {
            MockRepository repository = new MockRepository(MockBehavior.Strict);
            Mock<IMessage> messageMock = repository.Create<IMessage>();

            string address = "targetAddress";

            messageMock.Setup<string>(m => m.Target).Returns(address);

            RouterImpl router = new RouterImpl();


            bool wasCalled = false;
            router.RegisterOrReplace(address, (m) => {wasCalled = true;});

            router.Send(messageMock.Object);

            messageMock.VerifyAll();
            Assert.True(wasCalled);
        }
    }
}
