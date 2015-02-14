using NUnit.Framework;
using SoftwareMind.SimpleWorkflow.Behaviours;

namespace SoftwareMind.SimpleWorkflow.Tests.UnitTests
{
    [TestFixture]
    public class WFStandardConnectorBehaviourTests : IWFConnectorBehaviourTests
    {
        public override void SetBehaviour()
        {
            this.behaviour = new WFStandardConnectorBehaviour();
        }
    }
}
