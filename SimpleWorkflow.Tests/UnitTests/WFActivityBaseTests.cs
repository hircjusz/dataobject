using System;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace SoftwareMind.SimpleWorkflow.Tests.UnitTests
{
    [TestFixture]
    public class WFActivityBaseTests
    {
        protected MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            this.mocks = new MockRepository();
        }

        [Test, Category("unit")]
        public void DoTransition_IncorrectState_ThrowsException()
        {
            new Action(() =>
            {
                WFActivityBase activity = mocks.PartialMock<WFActivityBase>();
                IWFActivityInstance activityInstance = mocks.Stub<IWFActivityInstance>();

                using (mocks.Record())
                {
                    activityInstance.State = WFActivityState.Initialized;
                }

                activity.DoTransition(activityInstance, "TEST");
            }).ShouldThrow<Exception>();
        }

        [Test, Category("unit")]
        public void DoTransition_ActivityCompletedNoConnectorSpecified_RunsAllConnectors()
        {
            WFActivityBase activity = mocks.PartialMock<WFActivityBase>();
            IWFActivityInstance activityInstance = mocks.Stub<IWFActivityInstance>();

            using (mocks.Record())
            {
                activityInstance.State = WFActivityState.Completed;
                activity.DoTransitions(null);
                LastCall.IgnoreArguments();
            }

            activity.DoTransition(activityInstance, null);
            activity.VerifyAllExpectations();
        }
    }
}
