using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SoftwareMind.SimpleWorkflow.Behaviours;

namespace SoftwareMind.SimpleWorkflow.Tests.UnitTests
{
    [TestFixture]
    public class WFConnectorTests
    {
        protected MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            this.mocks = new MockRepository();
        }

        [Test, Category("unit")]
        public void IsAvailable_NoConditionExist_ReturnsTrue()
        {
            IWFProcessInstance processInstance = mocks.Stub<IWFProcessInstance>();

            using (mocks.Record())
            {
                processInstance.GetVariables();
                LastCall.IgnoreArguments().Return(null);
            }

            WFConnector connector = new WFConnector(new WFStandardConnectorBehaviour());
            connector.ConditionScript = null;
            var result = connector.IsAvailable(processInstance, null);
            Assert.IsTrue(result);
        }

        [Test, Category("unit")]
        public void IsAvailable_OneConditionNotFulfilled_ReturnsFalse()
        {
            IWFCondition condition = mocks.Stub<IWFCondition>();
            IWFProcessInstance processInstance = mocks.Stub<IWFProcessInstance>();

            using (mocks.Record())
            {
                condition.Eval(null);
                LastCall.IgnoreArguments().Return(false);

                processInstance.GetVariables();
                LastCall.IgnoreArguments().Return(null);
            }

            WFConnector connector = new WFConnector(new WFStandardConnectorBehaviour());
            connector.Conditions = new List<IWFCondition>() { condition };

            var result = connector.IsAvailable(processInstance, null);
            Assert.IsFalse(result);
        }

        [Test, Category("unit")]
        public void Join_NoJoinsExist_CreatesNewJoin()
        {
            WFActivityBase activityA = mocks.Stub<WFActivityBase>();
            WFActivityBase activityB = mocks.Stub<WFActivityBase>();

            IWFConnector connector = WFConnectionHelper.JoinActivities(activityA, activityB);
            Assert.IsTrue(activityA.ConnectorsOutgoing.Contains(connector));
            Assert.IsTrue(activityB.ConnectorsIncoming.Contains(connector));
        }
    }
}
