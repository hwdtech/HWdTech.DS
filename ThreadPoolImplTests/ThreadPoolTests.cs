using NUnit.Framework;

using HWdTech.DS.Internals.Implementation;

namespace HWdTech.DS.Internals.Implementation.Tests
{
    [TestFixture(Category = "Internals")]
    public class RunTasksTests
    {
        [Test]
        public void ThreadPoolShouldRunTasksWithoutAnyArguments()
        {
            System.Threading.ManualResetEvent canContinue = new System.Threading.ManualResetEvent(false);

            ThreadPoolImpl pool = new ThreadPoolImpl();

            bool argumentIsNull = false;

            pool.StartTask((obj) => {
                argumentIsNull = null == obj;
                canContinue.Set(); 
            });

            Assert.True(canContinue.WaitOne(1000));
            Assert.True(argumentIsNull);
        }

        [Test]
        public void ThreadPoolShouldRunTasksWithArgument()
        {
            System.Threading.ManualResetEvent canContinue = new System.Threading.ManualResetEvent(false);

            ThreadPoolImpl pool = new ThreadPoolImpl();

            object o = new object();
            bool gotArgument = false;

            pool.StartTask((obj) => {
                gotArgument = object.ReferenceEquals(obj, o);
                canContinue.Set(); 
            }, o);

            Assert.True(canContinue.WaitOne(1000));
            Assert.True(gotArgument);
        }
    }
}
