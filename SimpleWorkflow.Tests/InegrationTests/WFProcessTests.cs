using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SoftwareMind.SimpleWorkflow.Activities;
using SoftwareMind.SimpleWorkflow.Conditions;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Tests.InegrationTests
{
    [TestFixture]
    public class WFProcessTests
    {
        [TearDown]
        public void TearDown()
        {
            WFProcessFactory.processes.Clear();
        }

        [Test, Category("integration")]
        public void SimpleForkWithAutomaticSteps_Run_NoStepsLeftToExecute()
        {
            WFProcess process = new WFProcess();

            process.Name = "TestProcess";

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

            WFConnectionHelper.JoinActivities(start, fork).Code = "0";
            WFConnectionHelper.JoinActivities(fork, activityA).Code = "1";
            WFConnectionHelper.JoinActivities(fork, activityB).Code = "2";
            WFConnectionHelper.JoinActivities(activityA, join).Code = "3";
            IWFConnector connector = WFConnectionHelper.JoinActivities(activityB, join);
            connector.ConditionScript = " result = true;//>";
            WFCondition condition = new WFCondition();

            process.AddActivity(start);
            process.AddActivity(fork);
            process.AddActivity(activityA);
            process.AddActivity(activityB);
            process.AddActivity(join);

            process.RootActivity = start;

            WFProcessFactory.processes.Add("TestProcess", process);
            WFProcessInstance state = new WFProcessInstance(process);
            state.Box = new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(state);
            state = process.Run(state, null);
            state.ExecuteWorkList(/*null, null, (i) => { }*/);
            // nie ma juz wiecej kroków do wykonania
            Assert.AreEqual(state.GetActivities().Count(), 5);
        }

        [Test, Category("integration")]
        public void SimpleForkWithHumanSteps_Run_WaitsForCompletion()
        {
            WFStartActivity start = new WFStartActivity()
            {
                Code = "START"
            };

            WFFork fork = new WFFork()
            {
                Code = "FORK"
            };

            WFActivityBase activityA = new WFManualActivity()
            {
                Code = "STEPA"
            };

            WFActivityBase activityB = new WFManualActivity()
            {
                Code = "STEPB"
            };

            WFJoin join = new WFJoin()
            {
                Code = "JOIN"
            };


            WFProcess process = new WFProcess();
            process.Name = "test";

            WFConnectionHelper.JoinActivities(start, fork);
            WFConnectionHelper.JoinActivities(fork, activityA);
            WFConnectionHelper.JoinActivities(fork, activityB);
            WFConnectionHelper.JoinActivities(activityA, join);
            WFConnectionHelper.JoinActivities(activityB, join);

            process.AddActivity(start);
            process.AddActivity(fork);
            process.AddActivity(activityA);
            process.AddActivity(activityB);
            process.AddActivity(join);

            process.RootActivity = start;

            WFProcessFactory.processes.Add("test", process);
            WFProcessInstance state = new WFProcessInstance(process);
            state.Box = new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(state);
            state = process.Run(state, null);
            state.ExecuteWorkList(/*null, null, (i) => { }*/);

            string[] codes = (from IWFActivityInstance step in state.GetActivities() where step.State == WFActivityState.Waiting select step.Code).ToArray();

            Assert.AreEqual(2, codes.Length);
            Assert.IsTrue(codes.Contains("STEPA"));
            Assert.IsTrue(codes.Contains("STEPB"));
        }

        [Test, Category("integration")]
        public void SimpleProcessSerialization()
        {

            WFVariable variable = new WFVariable("ID", 12345);

            WFVariableDef varDefinition = new WFVariableDef()
            {
                Direction = WFVariableType.In,
                IsCollection = false,
                Name = "ID",
                Type = typeof(long)
            };

            WFVariableDef varDefinition2 = new WFVariableDef()
            {
                Direction = WFVariableType.In,
                IsCollection = true,
                Name = "List",
                Type = typeof(string)
            };

            IWFCondition condition1 = new WFCondition()
            {
                Expression = "if( 3 > 5) result = false;"
            };

            IWFCondition condition2 = new WFCondition()
            {
                Expression = "if( 2 > 5) result = false;"
            };

            WFActivityMetaData md = new WFActivityMetaData()
            {
                Key = "STAGE",
                Value = "BUSINESS"
            };

            WFStartActivity start = new WFStartActivity()
            {
                Code = "START"
            };

            WFFork fork = new WFFork()
            {
                Code = "FORK"
            };

            WFActivityBase activityA = new WFManualActivity()
            {
                Code = "STEPA"
            };

            activityA.MetaData.Add(md);

            WFActivityBase activityB = new WFManualActivity()
            {
                Code = "STEPB"
            };

            activityB.MetaData.Add(md);

            WFJoin join = new WFJoin()
            {
                Code = "JOIN"
            };

            WFProcess process = new WFProcess();
            IWFConnector connector = null;
            connector = WFConnectionHelper.JoinActivities(start, fork);
            connector = WFConnectionHelper.JoinActivities(fork, activityA);
            connector = WFConnectionHelper.JoinActivities(fork, activityB);

            connector = WFConnectionHelper.JoinActivities(activityA, join);
            connector.Code = "Z1";
            connector.Caption = "Zamknij";
            connector.ConditionScript = "test = 2 + 3;";
            connector.PolicyScript = "if( 2 > 5 ) throw new Exception();";

            connector = WFConnectionHelper.JoinActivities(activityB, join);
            process.AddActivity(start);
            process.AddActivity(fork);
            process.AddActivity(activityA);
            process.AddActivity(activityB);
            process.AddActivity(join);
            process.Name = "Simple";
            WFProcessFactory.processes.Clear();
            WFProcessFactory.processes.Add("Simple", process);
            process.VariableDefs.Add(varDefinition);
            process.VariableDefs.Add(varDefinition2);

            process.RootActivity = start;
            string processXml = ProcessTemplate.SaveToXml(process);
            WFProcessInstance instance = new WFProcessInstance(process);
            instance.Box = new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(instance);

            instance = process.Run(instance, new Dictionary<string, object>()
            {
                { "ID", 123456 },
                { "List", new List<string>() { "test", "test2" } }
            });
            instance.ExecuteWorkList(/*null,null, (i) => { }*/);
            string test = instance.WriteStateToXmlElement().ToString();
        }

        [Test, Category("integration")]
        public void SimpleProcessSerializationDeserialization()
        {
            WFStartActivity start = new WFStartActivity()
            {
                Code = "START"
            };

            WFFork fork = new WFFork()
            {
                Code = "FORK"
            };

            WFActivityBase activityA = new WFManualActivity()
            {
                Code = "STEPA"
            };


            WFActivityBase activityB = new WFManualActivity()
            {
                Code = "STEPB"
            };


            WFJoin join = new WFJoin()
            {
                Code = "JOIN"
            };

            WFProcess process = new WFProcess();

            WFConnectionHelper.JoinActivities(start, fork);
            WFConnectionHelper.JoinActivities(fork, activityA);
            WFConnectionHelper.JoinActivities(fork, activityB);
            WFConnectionHelper.JoinActivities(activityA, join);
            WFConnectionHelper.JoinActivities(activityB, join);

            process.AddActivity(start);
            process.AddActivity(fork);
            process.AddActivity(activityA);
            process.AddActivity(activityB);
            process.AddActivity(join);

            process.Name = "Simple";
            process.RootActivity = start;
            string processXml = ProcessTemplate.SaveToXml(process);
            WFProcess deserializedProcess = ProcessTemplate.LoadFromXml(processXml);

            Assert.AreEqual("Simple", deserializedProcess.Name);
            IEnumerable<string> activities = deserializedProcess.GetAllActivities().Select(x => x.Code);
            Assert.AreEqual(5, deserializedProcess.GetAllActivities().Count());
            Assert.IsTrue(activities.Contains("START"));
            Assert.IsTrue(activities.Contains("STEPB"));
            Assert.IsTrue(activities.Contains("JOIN"));
            Assert.IsTrue(activities.Contains("STEPA"));
            Assert.IsTrue(activities.Contains("FORK"));

        }

        [Test, Category("integration")]
        public void Multiplier_1ActivityMultipliedTwice_FullRun()
        {
            WFStartActivity start = new WFStartActivity()
            {
                Code = "START"
            };

            WFActivityMultiplicator multi = new WFActivityMultiplicator()
            {
                Code = "Multiply",
                MultiplicatorInstance = new ListSplitter() { ListVariableName = "List" }
            };

            multi.VariableDefs.Add("List", new WFVariableDef("List", typeof(string), true));

            WFActivityBase activityA = new WFManualActivity()
            {
                Code = "STEPA"
            };

            WFEndActivity end = new WFEndActivity()
            {
                Code = "END"
            };

            WFProcess process = new WFProcess();
            WFConnectionHelper.JoinActivities(start, multi);
            WFConnectionHelper.JoinActivities(multi, activityA);
            WFConnectionHelper.JoinActivities(activityA, end);

            process.AddActivity(start);
            process.AddActivity(multi);
            process.AddActivity(activityA);
            process.AddActivity(end);

            process.Name = "Simple";
            process.RootActivity = start;

            WFProcessFactory.processes.Add("Simple", process);

            IDictionary<string, object> args = new Dictionary<string, object>();
            args.Add("List", new string[] { "t1", "t2" });

            IWFProcessInstance instance = WFProcessCoordinator.Instance.RunProcess(new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(null), "Simple", args);

            string[] completedCodes = null;

            completedCodes = (from activity in instance.GetActivities()
                              where activity.State == WFActivityState.Completed
                              select activity.Code).ToArray();

            Assert.AreEqual(2, completedCodes.Length);
            Assert.IsTrue(completedCodes.Contains("START"));
            Assert.IsTrue(completedCodes.Contains("Multiply"));

            IWFTransitionResult result = WFProcessCoordinator.Instance.DoTransition(new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(instance), "STEPA|1#Multiply", null, null);
            completedCodes = (from activity in result.ProcessInstace.GetActivities() where activity.State == WFActivityState.Completed select activity.Code).ToArray();

            Assert.AreEqual(4, completedCodes.Length);
            Assert.IsTrue(completedCodes.Contains("START"));
            Assert.IsTrue(completedCodes.Contains("Multiply"));
            Assert.IsTrue(completedCodes.Contains("STEPA|1#Multiply"));
        }


        [Test, Category("integration")]
        public void AbortTest()
        {
            // Szkic procesu:
            //
            //            C_
            //            .  \
            //      Abort .    \
            //            .     v
            //   S ----> A ---> B ----> E
            //
            WFProcess process = new WFProcess();
            WFStartActivity s = new WFStartActivity()
            {
                Code = "S",
            };
            WFEndActivity e = new WFEndActivity()
            {
                Code = "E",
            };
            WFManualActivity a = new WFManualActivity()
            {
                Code = "A",
            };
            WFManualActivity b = new WFManualActivity()
            {
                Code = "B",
            };
            WFHandleEventActivity c = new WFHandleEventActivity()
            {
                Code = "C",
                ConditionScript = @"result = instance.IsWaiting(""A"");",
            };
            process.RootActivity = s;
            process.AddActivity(s);
            process.AddActivity(a);
            process.AddActivity(b);
            process.AddActivity(c);
            process.AddActivity(e);
            WFConnectionHelper.JoinActivities(s, a).Code = "sa";
            WFConnectionHelper.JoinActivities(a, b).Code = "ab";
            WFConnectionHelper.JoinActivities(b, e).Code = "be";
            WFConnectionHelper.JoinActivities(c, b).Code = "cb";
            WFConnectionHelper.JoinActivities(c, a, Behaviours.WFConnectorBehaviourType.Abort).Code = "ca";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            WFProcessInstance instance = new WFProcessInstance(process);
            WFProcessCoordinator.Instance.RunProcess(process, dic, null, new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(instance));
            var si = instance.GetActivity("S");
            var ai = instance.GetActivity("A");
            var bi = instance.GetActivity("B");
            var ci = instance.GetActivity("C");
            var ei = instance.GetActivity("E");

            Assert.AreEqual(WFActivityState.Completed, si.State);
            Assert.AreEqual(WFActivityState.Waiting, ai.State);
            Assert.AreEqual(WFActivityState.Initialized, bi.State);
            Assert.AreEqual(WFActivityState.Initialized, ei.State);
            Assert.IsTrue(c.IsAvailable(instance));

            WFProcessCoordinator.Instance.FireEventByActivityCode(new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(instance), "C", dic);

            Assert.AreEqual(WFActivityState.Completed, si.State);
            Assert.AreEqual(WFActivityState.Initialized, ai.State);
            Assert.AreEqual(WFActivityState.Waiting, bi.State);
            Assert.AreEqual(WFActivityState.Initialized, ei.State);
            Assert.IsFalse(c.IsAvailable(instance));
        }

        [Test, Category("integration")]
        public void AbortWaitForEventTest()
        {
            // Szkic procesu:
            //
            //                E1 ---> B
            //                .        \
            //          Abort .         \
            //                .          |
            //                V          V
            //  S ---> A ---> W          E
            //                ^          ^
            //                .          |
            //          Abort .         /
            //                .        /
            //                E2 ---> C
            WFProcess process = new WFProcess();
            WFStartActivity s = new WFStartActivity()
            {
                Code = "S",
            };
            WFEndActivity e = new WFEndActivity()
            {
                Code = "E",
            };
            WFManualActivity a = new WFManualActivity()
            {
                Code = "A",
            };
            WFManualActivity b = new WFManualActivity()
            {
                Code = "B",
            };
            WFManualActivity c = new WFManualActivity()
            {
                Code = "C",
            };
            WFWaitForEventActivity w = new WFWaitForEventActivity()
            {
                Code = "W",
            };
            WFHandleEventActivity e1 = new WFHandleEventActivity()
            {
                Code = "E1",
                ConditionScript = @"result = instance.IsWaiting(""W"");",
            };
            WFHandleEventActivity e2 = new WFHandleEventActivity()
            {
                Code = "E2",
                ConditionScript = @"result = instance.IsWaiting(""W"");",
            };
            process.RootActivity = s;
            process.AddActivity(s);
            process.AddActivity(a);
            process.AddActivity(b);
            process.AddActivity(c);
            process.AddActivity(e);
            process.AddActivity(e1);
            process.AddActivity(e2);
            process.AddActivity(w);
            WFConnectionHelper.JoinActivities(s, a).Code = "sa";
            WFConnectionHelper.JoinActivities(a, w).Code = "aw";
            WFConnectionHelper.JoinActivities(b, e).Code = "be";
            WFConnectionHelper.JoinActivities(c, e).Code = "ce";
            WFConnectionHelper.JoinActivities(e1, b).Code = "e1b";
            WFConnectionHelper.JoinActivities(e2, c).Code = "e2c";
            WFConnectionHelper.JoinActivities(e1, w, Behaviours.WFConnectorBehaviourType.Abort).Code = "e1w";
            WFConnectionHelper.JoinActivities(e2, w, Behaviours.WFConnectorBehaviourType.Abort).Code = "e2w";


            Dictionary<string, object> dic = new Dictionary<string, object>();
            WFProcessInstance instance = new WFProcessInstance(process);
            WFProcessCoordinator.Instance.RunProcess(process, dic, null, new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(instance));
            var si = instance.GetActivity("S");
            var ai = instance.GetActivity("A");
            var bi = instance.GetActivity("B");
            var ci = instance.GetActivity("C");
            var ei = instance.GetActivity("E");
            var e1i = instance.GetActivity("E1");
            var e2i = instance.GetActivity("E2");
            var wi = instance.GetActivity("W");

            Assert.AreEqual(WFActivityState.Completed, si.State);
            Assert.AreEqual(WFActivityState.Waiting, ai.State);
            Assert.AreEqual(WFActivityState.Initialized, wi.State);
            Assert.AreEqual(WFActivityState.Initialized, ei.State);
            Assert.AreEqual(WFActivityState.Initialized, bi.State);
            Assert.AreEqual(WFActivityState.Initialized, ci.State);
            Assert.IsFalse(e1.IsAvailable(instance));
            Assert.IsFalse(e2.IsAvailable(instance));

            WFProcessCoordinator.Instance.DoTransition(new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(instance), "A", "aw", dic);

            Assert.AreEqual(WFActivityState.Completed, si.State);
            Assert.AreEqual(WFActivityState.Completed, ai.State);
            Assert.AreEqual(WFActivityState.Waiting, wi.State);
            Assert.AreEqual(WFActivityState.Initialized, ei.State);
            Assert.AreEqual(WFActivityState.Initialized, bi.State);
            Assert.AreEqual(WFActivityState.Initialized, ci.State);
            Assert.IsTrue(e1.IsAvailable(instance));
            Assert.IsTrue(e2.IsAvailable(instance));

            WFProcessCoordinator.Instance.FireEventByActivityCode(new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(instance), "E1", dic);

            Assert.AreEqual(WFActivityState.Completed, si.State);
            Assert.AreEqual(WFActivityState.Completed, ai.State);
            Assert.AreEqual(WFActivityState.Initialized, wi.State);
            Assert.AreEqual(WFActivityState.Initialized, ei.State);
            Assert.AreEqual(WFActivityState.Waiting, bi.State);
            Assert.AreEqual(WFActivityState.Initialized, ci.State);
            Assert.IsFalse(e1.IsAvailable(instance));
            Assert.IsFalse(e2.IsAvailable(instance));
        }

        [Test, Category("integration")]
        public void ChechIfManualActivityIsNotClosedAfterTwiceEnteringTest()
        {
            // Szkic procesu:
            //
            //         E1
            //         |
            //         |.
            //         |
            //         v
            //  S ---> A ---> E
            //
            WFProcess process = new WFProcess();
            WFStartActivity s = new WFStartActivity()
            {
                Code = "S",
            };
            WFEndActivity e = new WFEndActivity()
            {
                Code = "E",
            };
            WFManualActivity a = new WFManualActivity()
            {
                Code = "A",
            };
            WFHandleEventActivity e1 = new WFHandleEventActivity()
            {
                Code = "E1"
            };
            process.RootActivity = s;
            process.AddActivity(s);
            process.AddActivity(a);
            process.AddActivity(e1);
            process.AddActivity(e);
            WFConnectionHelper.JoinActivities(s, a).Code = "sa";
            WFConnectionHelper.JoinActivities(e1, a).Code = "e1a";
            WFConnectionHelper.JoinActivities(a, e).Code = "ae";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            WFProcessInstance instance = new WFProcessInstance(process);
            WFProcessCoordinator.Instance.RunProcess(process, dic, null, new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(instance));
            var si = instance.GetActivity("S");
            var ai = instance.GetActivity("A");
            var e1i = instance.GetActivity("E1");
            var ei = instance.GetActivity("E");

            Assert.AreEqual(WFActivityState.Completed, si.State);
            Assert.AreEqual(WFActivityState.Waiting, ai.State);
            Assert.AreEqual(WFActivityState.Initialized, ei.State);

            WFProcessCoordinator.Instance.FireEventByActivityCode(new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(instance), "E1", dic);

            Assert.AreEqual(WFActivityState.Completed, si.State);
            Assert.AreEqual(WFActivityState.Waiting, ai.State);
            Assert.AreEqual(WFActivityState.Initialized, ei.State);
        }

        private static void EmptyAction(object o)
        {

        }

    }
}
