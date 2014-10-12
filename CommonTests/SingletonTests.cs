using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace HWdTech.Common.Tests
{
    [TestFixture]
    public class SingletonTests
    {
        class TheClassMeetsSingletonRequirements
        {
            static bool wasInitialized = false;
            public static void Init()
            {
                Singleton<TheClassMeetsSingletonRequirements>.Instance = new TheClassMeetsSingletonRequirements();
                wasInitialized = true;
            }

            public static void VerifyThatShouldBeInitialized()
            {
                Assert.IsTrue(wasInitialized);
            }

        }

        [Test]
        public void SingletonShoulUseStaticInitMethodToInitializeInstance()
        {
            Assert.IsNotNull(Singleton<TheClassMeetsSingletonRequirements>.Instance);
            TheClassMeetsSingletonRequirements.VerifyThatShouldBeInitialized();
        }

        [Test]
        public void SingletonShouldProvideALinkToSingleObject()
        {
            Assert.AreSame(Singleton<TheClassMeetsSingletonRequirements>.Instance, Singleton<TheClassMeetsSingletonRequirements>.Instance);
        }

        public class TheClassDoesNotMeetSingletonRequirements
        {
        }

        [Test]
        public void SingletonShouldThrowExceptionIfASingletonObjectHasNoInitMethod()
        {
            Assert.Throws<Exception>(() => { object o = Singleton<TheClassDoesNotMeetSingletonRequirements>.Instance; });
        }
    }
}
