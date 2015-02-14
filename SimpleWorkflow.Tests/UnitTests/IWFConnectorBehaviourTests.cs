using System;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace SoftwareMind.SimpleWorkflow.Tests.UnitTests
{
    [TestFixture]
    public abstract class IWFConnectorBehaviourTests
    {
        protected MockRepository mocks;
        protected IWFConnectorBehaviour behaviour;

        [SetUp]
        public void SetUp()
        {
            this.mocks = new MockRepository();
            this.SetBehaviour();
        }

        public abstract void SetBehaviour();

        [Test, Category("unit")]
        public virtual void IsAvailable_NotConfigured_ThrowsException()
        {
            new Action(() =>
            {
                IWFProcessInstance instance = mocks.Stub<IWFProcessInstance>();
                behaviour.IsAvailable(instance, null);
            }).ShouldThrow<InvalidOperationException>();
        }

        [Test, Category("unit")]
        public virtual void Run_NotConfigured_ThrowsException()
        {
            new Action(() =>
            {
                IWFActivityInstance activityInstance = mocks.Stub<IWFActivityInstance>();
                behaviour.Run(activityInstance, null, null, false, null);
            }).ShouldThrow<InvalidOperationException>();
        }
    }
}
