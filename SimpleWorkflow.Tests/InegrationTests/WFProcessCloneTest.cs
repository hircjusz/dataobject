using System.Linq;
using NUnit.Framework;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Tests.InegrationTests
{
    [TestFixture]
    public class WFProcessCloneTest
    {
        [Test]
        public void SimpleProcess_Clone()
        {
            var process = this.LoadSimpleProcess();
            var clone = (WFProcess) process.Clone();

            Assert.AreEqual(process.Name, clone.Name);

            var root = process.RootActivity;
            var rootClone = clone.RootActivity;

            Assert.AreNotEqual(root, rootClone);
            Assert.AreEqual(root.Code, rootClone.Code);
            Assert.AreEqual(root.GetType(), rootClone.GetType());
            Assert.AreEqual(root.ConnectorsIncoming.Count, rootClone.ConnectorsIncoming.Count);
            Assert.AreEqual(root.ConnectorsOutgoing.Count, rootClone.ConnectorsOutgoing.Count);

            var middle = root.ConnectorsOutgoing.Single().Destination;
            var middleClone = rootClone.ConnectorsOutgoing.Single().Destination;

            Assert.AreNotEqual(middle, middleClone);
            Assert.AreEqual(middle.Code, middleClone.Code);
            Assert.AreEqual(middle.GetType(), middleClone.GetType());
            Assert.AreEqual(middle.ConnectorsIncoming.Count, middleClone.ConnectorsIncoming.Count);
            Assert.AreEqual(middle.ConnectorsOutgoing.Count, middleClone.ConnectorsOutgoing.Count);

            var finish = root.ConnectorsOutgoing.Single().Destination;
            var finishClone = rootClone.ConnectorsOutgoing.Single().Destination;

            Assert.AreNotEqual(finish, finishClone);
            Assert.AreEqual(finish.Code, finishClone.Code);
            Assert.AreEqual(finish.GetType(), finishClone.GetType());
            Assert.AreEqual(finish.ConnectorsIncoming.Count, finishClone.ConnectorsIncoming.Count);
            Assert.AreEqual(finish.ConnectorsOutgoing.Count, finishClone.ConnectorsOutgoing.Count);
        }

        private WFProcess LoadSimpleProcess()
        {
            const string xml = @"
                <Template Name=""Template1"" Version=""1"">
                  <Activities>
                    <Activity Code=""START"" Caption=""START"" Description=""START"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
                      <MetaDataCollection />
                    </Activity>
                    <Activity Code=""TEMPLATE"" Caption=""Template"" Description=""Template"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFScriptActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" Script="""" LongRunning=""false"">
                      <MetaDataCollection />
                    </Activity>
                    <Activity Code=""FINISH"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
                      <MetaDataCollection />
                    </Activity>
                  </Activities>
                  <Connectors>
                    <Connector From=""START"" To=""TEMPLATE"" Code=""START_TEMPLATE"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
                      <Conditions />
                      <Policies />
                      <MetaDataCollection />
                      <DialogBoxValueMaping />
                      <DialogBoxParameters />
                    </Connector>
                    <Connector From=""TEMPLATE"" To=""FINISH"" Code=""TEMPLATE_FINISH"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
                      <Conditions />
                      <Policies />
                      <MetaDataCollection />
                      <DialogBoxValueMaping />
                      <DialogBoxParameters />
                    </Connector>
                  </Connectors>
                </Template>
            ";

            return ProcessTemplate.LoadFromXml(xml);
        }
    }
}
