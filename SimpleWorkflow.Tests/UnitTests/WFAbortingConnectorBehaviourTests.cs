using NUnit.Framework;
using SoftwareMind.SimpleWorkflow.Behaviours;

namespace SoftwareMind.SimpleWorkflow.Tests.UnitTests
{
    [TestFixture]
    public class WFAbortingConnectorBehaviourTests : IWFConnectorBehaviourTests
    {
        public override void SetBehaviour()
        {
            this.behaviour = new WFAbortingConnectorBehaviour();
        }
    }
}
