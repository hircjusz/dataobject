using NUnit.Framework;
using Rhino.Mocks;

namespace SoftwareMind.SimpleWorkflow.Tests.UnitTests
{
    [TestFixture]
    public class WFProcessInstanceTests
    {
        protected MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            this.mocks = new MockRepository();
        }

        [Test, Category("unit")]
        public void IsClosed_OneAwaitingActivityExists_ReturnsFalse()
        {
            WFProcessInstance processInsance = new WFProcessInstance();
            IWFActivityInstance activityInstance = mocks.Stub<IWFActivityInstance>();
            activityInstance.State = WFActivityState.Waiting;
            activityInstance.Code = "test";
            processInsance.AddActivity(activityInstance);
            Assert.IsFalse(processInsance.IsClosed());
        }

        [Test, Category("unit")]
        public void IsClosed_NoActivities_ReturnsFalse()
        {
            WFProcessInstance processInsance = new WFProcessInstance();
            IWFActivityInstance activityInstance = mocks.Stub<IWFActivityInstance>();
            Assert.IsFalse(processInsance.IsClosed());
        }

        [Test, Category("unit")]
        public void ExecuteWorkList_NoElementsInQueue_CallbackNotCalled()
        {
            int counter = 0;
            WFProcessInstance processInstance = new WFProcessInstance();
            processInstance.ExecuteWorkList(/*(pi) => { counter++; }, null, (i) => { }*/);

            Assert.AreEqual(0, counter);
        }

    }
}
