using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SoftwareMind.SimpleWorkflow.Activities;

namespace SoftwareMind.SimpleWorkflow.Tests.UnitTests
{
    [TestFixture]
    public class WFStateMachineTests
    {
        #region Template
        private static readonly string template = @"
            <Template Name=""testTemplate"" Version="""" DesingerSettings=""1500,2000"">
              <Activities>
                <Activity Code=""C"" Caption=""CLOSED"" Description="""" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStateMachineEndActivity"" DesignerSettings=""55,605,40,45"" />
                <Activity Code=""X"" Caption=""CANCELLED"" Description="""" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStateMachineEndActivity"" DesignerSettings=""195,605,40,45"" />
                <Activity Code=""Q"" Caption=""REJECTED"" Description="""" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStateMachineActivity"" DesignerSettings=""55,185,40,70"" />
                <Activity Code=""R"" Caption=""RESOLVED"" Description="""" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStateMachineActivity"" DesignerSettings=""90,465,40,70"" />
                <Activity Code=""A"" Caption=""ASSIGNED"" Description="""" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStateMachineActivity"" DesignerSettings=""90,325,40,70"" />
                <Activity Code=""N"" Caption=""N"" Description="""" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStateMachineActivity"" DesignerSettings=""125,185,40,70"" />
                <Activity Code="""" Caption=""Start"" Description="""" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""125,70,40,45"" />
              </Activities>
              <Connectors>
                <Connector From=""Q"" To=""C"" Code=""Rejected_Close"" Caption=""Zamknij"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""66;255;40;360;40;500;68;605"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""Q"" To=""X"" Code=""Rejected_Cancel"" Caption=""Anuluj"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""95;247;180;360;250;500;221;605"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""Q"" To=""A"" Code=""Rejected_Assign"" Caption=""Przypisz ponownie"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""83;255;101;325"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""R"" To=""C"" Code=""Resolved_Close"" Caption=""Zamknij"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""100;535;81;605"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""R"" To=""X"" Code=""Resolved_Cancel"" Caption=""Anuluj"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""130;524;196;605"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""R"" To=""A"" Code=""Resolved_Assign"" Caption=""Przypisz ponownie"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""110;465;110;395"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""A"" To=""Q"" Code=""Assigned_Reject"" Caption=""Odrzuć"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""101;325;83;255"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""A"" To=""R"" Code=""Assigned_Resolve"" Caption=""Rozwiąż"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""110;395;110;465"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""A"" To=""X"" Code=""Assigned_Cancel"" Caption=""Anuluj"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""127;395;180;500;208;605"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""N"" To=""A"" Code=""New_Assign"" Caption=""Przypisz"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""136;255;118;325"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From=""N"" To=""X"" Code=""New_Cancel"" Caption=""Anuluj"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""165;247;250;360;320;500;233;605"">
                  <Conditions />
                  <Policies />
                </Connector>
                <Connector From="""" To=""N"" Code=""Start"" Caption="""" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour"" DesignerSettings=""145;115;145;185"">
                  <Conditions />
                  <Policies />
                </Connector>
              </Connectors>
            </Template>";

        #endregion

        private WFStateMachine machine;

        [SetUp]
        public void SetUp()
        {
            machine = WFStateMachine.LoadMachineFromXml(template);
        }

        #region Restore machine from template

        [Test, Category("integration")]
        public void RestoreMachineFromTemplate__AllConnectedActivities__Returns_7()
        {
            IEnumerable<WFActivityBase> states = machine.Process.GetAllActivities();

            Assert.AreEqual(7, states.Count());
        }

        [Test, Category("integration")]
        public void RestoreMachineFromTemplate__AllConnectedActivitiesTypes__Returns_1WFStartActivity_4WFStateMachineActivity_2WFStateMachineEndActivity()
        {
            IEnumerable<WFActivityBase> states = machine.Process.GetAllActivities();

            Assert.AreEqual(1, states.Count(s => s.GetType() == typeof(WFStartActivity)));
            Assert.AreEqual(4, states.Count(s => s.GetType() == typeof(WFStateMachineActivity)));
            Assert.AreEqual(2, states.Count(s => s.GetType() == typeof(WFStateMachineEndActivity)));
        }

        [Test, Category("integration")]
        public void RestoreMachineFromTemplate__AllConnectedActivitiesCodes__Returns_1Closed_1Cancelled_1Rejected_1Resolved_1Assigned_1NEW()
        {
            IEnumerable<WFActivityBase> states = machine.Process.GetAllActivities();

            Assert.AreEqual(1, states.Count(s => s.Code == "C"));
            Assert.AreEqual(1, states.Count(s => s.Code == "X"));
            Assert.AreEqual(1, states.Count(s => s.Code == "Q"));
            Assert.AreEqual(1, states.Count(s => s.Code == "R"));
            Assert.AreEqual(1, states.Count(s => s.Code == "A"));
            Assert.AreEqual(1, states.Count(s => s.Code == "N"));
        }

        #endregion

        #region Transitions

        #region Transitions from NEW

        [Test, Category("integration")]
        public void Transitions__New_To_Assigned__Returns_True()
        {
            WFActivityBase state = machine.DoTransition("N", "New_Assign");

            Assert.AreEqual("A", state.Code);
        }
        [Test, Category("integration")]
        public void Transitions__New_To_Cancelled__Returns_True()
        {
            WFActivityBase state = machine.DoTransition("N", "New_Cancel");

            Assert.AreEqual("X", state.Code);
        }

        #endregion

        #region Transitions from ASS

        [Test, Category("integration")]
        public void Transitions__Assigned_To_Resolved__Returns_True()
        {
            WFActivityBase state = machine.DoTransition("A", "Assigned_Resolve");

            Assert.AreEqual("R", state.Code);
        }
        [Test, Category("integration")]
        public void Transitions__Assigned_To_Rejected__Returns_True()
        {
            WFActivityBase state = machine.DoTransition("A", "Assigned_Reject");

            Assert.AreEqual("Q", state.Code);
        }
        [Test, Category("integration")]
        public void Transitions__Assigned_To_Cancelled__Returns_True()
        {
            WFActivityBase state = machine.DoTransition("A", "Assigned_Cancel");

            Assert.AreEqual("X", state.Code);
        }

        #endregion

        #region Available transitions

        [Test, Category("integration")]
        public void AvailableTransitions__New__Returns_Assigned_CAN()
        {
            IEnumerable<IWFConnector> states = machine.GetAvailableTransitions("N");
            Assert.AreEqual(2, states.Count());
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "A"));
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "X"));
        }

        [Test, Category("integration")]
        public void AvailableTransitions__Assigned__Returns_Resolved_Rejected_CAN()
        {
            IEnumerable<IWFConnector> states = machine.GetAvailableTransitions("A");
            Assert.AreEqual(3, states.Count());
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "R"));
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "Q"));
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "X"));
        }

        [Test, Category("integration")]
        public void AvailableTransitions__Resolved__Returns_Assigned_Closed_CAN()
        {
            IEnumerable<IWFConnector> states = machine.GetAvailableTransitions("R");
            Assert.AreEqual(3, states.Count());
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "A"));
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "C"));
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "X"));
        }
        [Test, Category("integration")]
        public void AvailableTransitions__Rejected__Returns_Assigned_Closed_CAN()
        {
            IEnumerable<IWFConnector> states = machine.GetAvailableTransitions("Q");
            Assert.AreEqual(3, states.Count());
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "A"));
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "C"));
            Assert.AreEqual(1, states.Count(s => s.Destination.Code == "X"));
        }

        [Test, Category("integration")]
        public void AvailableTransitions__Closed__Returns_None()
        {
            IEnumerable<IWFConnector> states = machine.GetAvailableTransitions("C");
            Assert.AreEqual(0, states.Count());
        }

        [Test, Category("integration")]
        public void AvailableTransitions__Cancelled__Returns_None()
        {
            IEnumerable<IWFConnector> states = machine.GetAvailableTransitions("X");
            Assert.AreEqual(0, states.Count());
        }

        #endregion

        #endregion
    }
}
