using System.Collections.Generic;
using NUnit.Framework;
using SoftwareMind.SimpleWorkflow.Activities;

namespace SoftwareMind.SimpleWorkflow.Tests.UnitTests
{
    [TestFixture]
    public class WFSubprocessTest
    {
        class WFTestPolicy : WFNullPolicyRule
        {
            internal IList<string> connectors = new List<string>();

            public override void Check(IWFProcessInstance processInstance, IDictionary<string, object> arguments)
            {
                connectors.Add(processInstance.TransitionConnector.Code);

                base.Check(processInstance, arguments);
            }
        }

        [TearDown]
        public void TearDown()
        {
            WFProcessFactory.processes.Clear();
        }

        [Test, Category("integration")]
        public void SimpleSubprocess()
        {
            WFProcess process = new WFProcess
            {
                Name = "TestProcess"
            };

            WFStartActivity start = new WFStartActivity()
            {
                Code = "START"
            };

            WFTestPolicy policy = new WFTestPolicy();
            WFSubprocess subprocess = new WFSubprocess()
            {
                Code = "SUB",
                SubProcess = this.CrateSubProcess(policy)
            };

            WFConnectionHelper.JoinActivities(start, subprocess).Code = "0";

            process.AddActivity(start);
            process.AddActivity(subprocess);

            process.RootActivity = start;

            WFProcessFactory.processes.Add("TestProcess", process);
            WFProcessInstance state = new WFProcessInstance(process);
            state.Box = new WFProcessInstance.TransparentBox(state);
            state = process.Run(state, null);
            state.ExecuteWorkList();

            Assert.AreEqual("0,1,2,3,4", string.Join(",", policy.connectors));
        }

        private WFProcess CrateSubProcess(IWFPolicyRule policy)
        {
            WFProcess process = new WFProcess();

            process.Name = "TestSubProcess";

            WFStartActivity start = new WFStartActivity()
            {
                Code = "START"
            };

            WFFork fork = new WFFork()
            {
                Code = "FORK"
            };

            WFActivityBase activityA = new WFWriteLineActivity()
            {
                Message = "Step A",
                Code = "STEP A"
            };

            WFActivityBase activityB = new WFWriteLineActivity()
            {
                Message = "Step B",
                Code = "STEP B"
            };

            WFJoin join = new WFJoin()
            {
                Code = "JOIN"
            };

            IWFConnector connector;

            connector = WFConnectionHelper.JoinActivities(start, fork);
            connector.Code = "0";
            connector.Policies = new List<IWFPolicyRule> { policy };

            connector = WFConnectionHelper.JoinActivities(fork, activityA);
            connector.Code = "1";
            connector.Policies = new List<IWFPolicyRule> { policy };

            connector = WFConnectionHelper.JoinActivities(fork, activityB);
            connector.Code = "2";
            connector.Policies = new List<IWFPolicyRule> { policy };

            connector = WFConnectionHelper.JoinActivities(activityA, join);
            connector.Code = "3";
            connector.Policies = new List<IWFPolicyRule> { policy };

            connector = WFConnectionHelper.JoinActivities(activityB, join);
            connector.Code = "4";
            connector.ConditionScript = "result = true;";
            connector.Policies = new List<IWFPolicyRule> { policy };

            process.AddActivity(start);
            process.AddActivity(fork);
            process.AddActivity(activityA);
            process.AddActivity(activityB);
            process.AddActivity(join);

            process.RootActivity = start;

            return process;
        }
    }
}
