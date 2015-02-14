//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Linq;
//using FluentAssertions;
//using NUnit.Framework;
//using Shared.Tests.Infrastructure;
//using SoftwareMind.SimpleWorkflow.Designer;
//using SoftwareMind.SimpleWorkflow.Misc;
//using SoftwareMind.Utils.Exceptions;

//namespace SoftwareMind.SimpleWorkflow.Tests.InegrationTests
//{
//    [Serializable]
//    [TestFixture]
//    public class WFProcessTemplateTest : PlatformTest
//    {
//        protected override IEnumerable<string> AssemblyCatalogs
//        {
//            get
//            {
//                return base.AssemblyCatalogs.Union(new[] {
//                    "SoftwareMind.Shared",
//                    "SoftwareMind.SimpleWorkflow"
//                });
//            }
//        }

//        [SetUp]
//        public new void SetUp()
//        {
//            base.SetUp();
//            this.PreloadTemplates();
//        }

//        [TearDown]
//        public override void TearDown()
//        {
//            base.TearDown();
//            WFProcessFactory.ClearCache();
//        }

//        [Test]
//        public void TemplateInclude_SingleTemplate_ValidTemplate()
//        {
//            var process = this.LoadSimpleProcess("Template1");

//            Assert.AreEqual(7, process.GetAllActivities().Count());

//            WFActivityBase activity = process.RootActivity;
//            Assert.AreEqual("START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TPL_START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TPL_START_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TEMPLATE", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TEMPLATE_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$FINISH", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TPL_END_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TPL_END", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("FINISH", activity.Code);
//            Assert.AreEqual(0, activity.ConnectorsOutgoing.Count);
//        }

//        [Test]
//        public void TemplateInclude_SingleTemplate_ValidTemplateWithMultipleConnectors()
//        {
//            var process = this.LoadSimpleProcess("Template3");

//            Assert.AreEqual(7, process.GetAllActivities().Count());

//            WFActivityBase activity = process.RootActivity;
//            Assert.AreEqual("START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector)activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TPL_START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TPL_START_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector)activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector)activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TEMPLATE", activity.Code);
//            Assert.AreEqual(2, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual(((WFConnector)activity.ConnectorsOutgoing.First()).Destination, ((WFConnector)activity.ConnectorsOutgoing.Skip(1).First()).Destination);
//            Assert.AreEqual("TEMPLATE$TEMPLATE_FINISH1", activity.ConnectorsOutgoing.First().Code);
//            Assert.AreEqual("TEMPLATE$TEMPLATE_FINISH2", activity.ConnectorsOutgoing.Skip(1).First().Code);

//            activity = ((WFConnector)activity.ConnectorsOutgoing.First()).Destination;
//            Assert.AreEqual("TEMPLATE$FINISH", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TPL_END_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector)activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TPL_END", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector)activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("FINISH", activity.Code);
//            Assert.AreEqual(0, activity.ConnectorsOutgoing.Count);
//        }

//        [Test]
//        public void TemplateInclude_MultipleTemplate_ValidTemplate()
//        {
//            var process = this.LoadSimpleProcessWithTwoTemplates("Template1", "Template1");

//            Assert.AreEqual(12, process.GetAllActivities().Count());

//            WFActivityBase activity = process.RootActivity;
//            Assert.AreEqual("START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("START_TEMPLATE1", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE1$TPL_START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE1$TPL_START_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE1$START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE1$START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE1$TEMPLATE", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE1$TEMPLATE_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE1$FINISH", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE1$TPL_END_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE1$TPL_END", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE1_TEMPLATE2", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE2$TPL_START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE2$TPL_START_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE2$START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE2$START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE2$TEMPLATE", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE2$TEMPLATE_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE2$FINISH", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE2$TPL_END_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE2$TPL_END", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE2_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("FINISH", activity.Code);
//            Assert.AreEqual(0, activity.ConnectorsOutgoing.Count);
//        }

//        [Test]
//        public void TemplateInclude_SingleNestedTemplate_ValidTemplate()
//        {
//            var process = this.LoadSimpleProcess("Template2");

//            Assert.AreEqual(11, process.GetAllActivities().Count());

//            WFActivityBase activity = process.RootActivity;
//            Assert.AreEqual("START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TPL_START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TPL_START_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TEMPLATE$TPL_START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TEMPLATE$TPL_START_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TEMPLATE$START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TEMPLATE$START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TEMPLATE$TEMPLATE", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TEMPLATE$TEMPLATE_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TEMPLATE$FINISH", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TEMPLATE$TPL_END_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TEMPLATE$TPL_END", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TEMPLATE_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$FINISH", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TPL_END_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TPL_END", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("FINISH", activity.Code);
//            Assert.AreEqual(0, activity.ConnectorsOutgoing.Count);
//        }

//        [Test]
//        public void TemplateInclude_ProcessWithTwoOutgoingConnectors()
//        {
//            new Action(() => this.LoadSimpleProcessWithTwoOutgoingConnectors("Template1"))
//                .ShouldThrow<ValidationException>()
//                .WithMessage("Template must have only one outgoing connector");
//        }

//        [Test]
//        public void TemplateInclude_ProcessWithTwoIncomingConnectors()
//        {
//            new Action(() => this.LoadSimpleProcessWithTwoIncomingConnectors("Template1"))
//                .ShouldThrow<ValidationException>()
//                .WithMessage("Template must have only one incoming connector");
//        }

//        [Test]
//        public void TemplateInclude_TemplateWithTwoEndActivities()
//        {
//            var process = this.LoadSimpleProcess("Template4");

//            Assert.AreEqual(8, process.GetAllActivities().Count());

//            WFActivityBase activity = process.RootActivity;
//            Assert.AreEqual("START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector)activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TPL_START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TPL_START_CONN", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector)activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$START", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$START_TEMPLATE", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector)activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("TEMPLATE$TEMPLATE", activity.Code);
//            Assert.AreEqual(2, activity.ConnectorsOutgoing.Count);
//            Assert.AreNotEqual(((WFConnector) activity.ConnectorsOutgoing.First()).Destination, ((WFConnector) activity.ConnectorsOutgoing.Skip(1).First()).Destination);
//            Assert.AreEqual("TEMPLATE$TEMPLATE_FINISH1", activity.ConnectorsOutgoing.First().Code);
//            Assert.AreEqual("TEMPLATE$TEMPLATE_FINISH2", activity.ConnectorsOutgoing.Skip(1).First().Code);

//            // First path
//            var fpActivity = ((WFConnector) activity.ConnectorsOutgoing.First()).Destination;
//            Assert.AreEqual("TEMPLATE$FINISH1", fpActivity.Code);
//            Assert.AreEqual(1, fpActivity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TPL_END_CONN", fpActivity.ConnectorsOutgoing.Single().Code);

//            // Second path
//            var spActivity = ((WFConnector) activity.ConnectorsOutgoing.Skip(1).First()).Destination;
//            Assert.AreEqual("TEMPLATE$FINISH2", spActivity.Code);
//            Assert.AreEqual(1, spActivity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE$TPL_END_CONN", spActivity.ConnectorsOutgoing.Single().Code);

//            Assert.AreEqual(((WFConnector) fpActivity.ConnectorsOutgoing.First()).Destination, ((WFConnector) spActivity.ConnectorsOutgoing.First()).Destination);

//            activity = ((WFConnector) fpActivity.ConnectorsOutgoing.First()).Destination;
//            Assert.AreEqual("TEMPLATE$TPL_END", activity.Code);
//            Assert.AreEqual(1, activity.ConnectorsOutgoing.Count);
//            Assert.AreEqual("TEMPLATE_FINISH", activity.ConnectorsOutgoing.Single().Code);

//            activity = ((WFConnector) activity.ConnectorsOutgoing.Single()).Destination;
//            Assert.AreEqual("FINISH", activity.Code);
//            Assert.AreEqual(0, activity.ConnectorsOutgoing.Count);
//        }

//        [Test]
//        public void ProcessRun_SingleTemplate_ValidTemplate()
//        {
//            var process = this.LoadSimpleProcess("Template1");
//            var steps = new List<Transition>();
//            var policy = new TestPolicy(steps);

//            foreach (var activity in process.GetAllActivities())
//            {
//                foreach (var connector in activity.ConnectorsOutgoing)
//                {
//                    connector.DesingerPolicies.Add(new DesingerPolicyItem {Policy = policy});
//                }
//            }

//            WFProcessInstance instance = new WFProcessInstance(process);
//            instance.Box = new WFProcessInstance.TransparentBox(instance);

//            instance = process.Run(instance, new Dictionary<string, object>());
//            instance.ExecuteWorkListSynchronously();

//            Assert.AreEqual(6, steps.Count);
//            Assert.AreEqual("START-(START_TEMPLATE)->TEMPLATE$TPL_START", steps[0].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_START-(TEMPLATE$TPL_START_CONN)->TEMPLATE$START", steps[1].ToString());
//            Assert.AreEqual("TEMPLATE$START-(TEMPLATE$START_TEMPLATE)->TEMPLATE$TEMPLATE", steps[2].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE-(TEMPLATE$TEMPLATE_FINISH)->TEMPLATE$FINISH", steps[3].ToString());
//            Assert.AreEqual("TEMPLATE$FINISH-(TEMPLATE$TPL_END_CONN)->TEMPLATE$TPL_END", steps[4].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_END-(TEMPLATE_FINISH)->FINISH", steps[5].ToString());
//        }

//        [Test]
//        public void ProcessRun_SingleTemplate_ValidTemplateWithMultipleConnectors()
//        {
//            var process = this.LoadSimpleProcess("Template3");
//            var steps = new List<Transition>();
//            var policy = new TestPolicy(steps);

//            foreach (var activity in process.GetAllActivities())
//            {
//                foreach (var connector in activity.ConnectorsOutgoing)
//                {
//                    connector.DesingerPolicies.Add(new DesingerPolicyItem {Policy = policy});
//                }
//            }

//            WFProcessInstance instance = new WFProcessInstance(process);
//            instance.Box = new WFProcessInstance.TransparentBox(instance);

//            instance = process.Run(instance, new Dictionary<string, object>());
//            instance.ExecuteWorkListSynchronously();

//            Assert.AreEqual(7, steps.Count);
//            Assert.AreEqual("START-(START_TEMPLATE)->TEMPLATE$TPL_START", steps[0].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_START-(TEMPLATE$TPL_START_CONN)->TEMPLATE$START", steps[1].ToString());
//            Assert.AreEqual("TEMPLATE$START-(TEMPLATE$START_TEMPLATE)->TEMPLATE$TEMPLATE", steps[2].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE-(TEMPLATE$TEMPLATE_FINISH1)->TEMPLATE$FINISH", steps[3].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE-(TEMPLATE$TEMPLATE_FINISH2)->TEMPLATE$FINISH", steps[4].ToString());
//            Assert.AreEqual("TEMPLATE$FINISH-(TEMPLATE$TPL_END_CONN)->TEMPLATE$TPL_END", steps[5].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_END-(TEMPLATE_FINISH)->FINISH", steps[6].ToString());
//        }

//        [Test]
//        public void ProcessRun_MultipleTemplate_ValidTemplate()
//        {
//            var process = this.LoadSimpleProcessWithTwoTemplates("Template1", "Template1");
//            var steps = new List<Transition>();
//            var policy = new TestPolicy(steps);

//            foreach (var activity in process.GetAllActivities())
//            {
//                foreach (var connector in activity.ConnectorsOutgoing)
//                {
//                    connector.DesingerPolicies.Add(new DesingerPolicyItem {Policy = policy});
//                }
//            }

//            WFProcessInstance instance = new WFProcessInstance(process);
//            instance.Box = new WFProcessInstance.TransparentBox(instance);

//            instance = process.Run(instance, new Dictionary<string, object>());
//            instance.ExecuteWorkListSynchronously();

//            Assert.AreEqual(11, steps.Count);
//            Assert.AreEqual("START-(START_TEMPLATE1)->TEMPLATE1$TPL_START", steps[0].ToString());
//            Assert.AreEqual("TEMPLATE1$TPL_START-(TEMPLATE1$TPL_START_CONN)->TEMPLATE1$START", steps[1].ToString());
//            Assert.AreEqual("TEMPLATE1$START-(TEMPLATE1$START_TEMPLATE)->TEMPLATE1$TEMPLATE", steps[2].ToString());
//            Assert.AreEqual("TEMPLATE1$TEMPLATE-(TEMPLATE1$TEMPLATE_FINISH)->TEMPLATE1$FINISH", steps[3].ToString());
//            Assert.AreEqual("TEMPLATE1$FINISH-(TEMPLATE1$TPL_END_CONN)->TEMPLATE1$TPL_END", steps[4].ToString());
//            Assert.AreEqual("TEMPLATE1$TPL_END-(TEMPLATE1_TEMPLATE2)->TEMPLATE2$TPL_START", steps[5].ToString());
//            Assert.AreEqual("TEMPLATE2$TPL_START-(TEMPLATE2$TPL_START_CONN)->TEMPLATE2$START", steps[6].ToString());
//            Assert.AreEqual("TEMPLATE2$START-(TEMPLATE2$START_TEMPLATE)->TEMPLATE2$TEMPLATE", steps[7].ToString());
//            Assert.AreEqual("TEMPLATE2$TEMPLATE-(TEMPLATE2$TEMPLATE_FINISH)->TEMPLATE2$FINISH", steps[8].ToString());
//            Assert.AreEqual("TEMPLATE2$FINISH-(TEMPLATE2$TPL_END_CONN)->TEMPLATE2$TPL_END", steps[9].ToString());
//            Assert.AreEqual("TEMPLATE2$TPL_END-(TEMPLATE2_FINISH)->FINISH", steps[10].ToString());
//        }

//        [Test]
//        public void ProcessRun_SingleNestedTemplate_ValidTemplate()
//        {
//            var process = this.LoadSimpleProcess("Template2");
//            var steps = new List<Transition>();
//            var policy = new TestPolicy(steps);

//            foreach (var activity in process.GetAllActivities())
//            {
//                foreach (var connector in activity.ConnectorsOutgoing)
//                {
//                    connector.DesingerPolicies.Add(new DesingerPolicyItem { Policy = policy });
//                }
//            }

//            WFProcessInstance instance = new WFProcessInstance(process);
//            instance.Box = new WFProcessInstance.TransparentBox(instance);

//            instance = process.Run(instance, new Dictionary<string, object>());
//            instance.ExecuteWorkListSynchronously();

//            Assert.AreEqual(10, steps.Count);
//            Assert.AreEqual("START-(START_TEMPLATE)->TEMPLATE$TPL_START", steps[0].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_START-(TEMPLATE$TPL_START_CONN)->TEMPLATE$START", steps[1].ToString());
//            Assert.AreEqual("TEMPLATE$START-(TEMPLATE$START_TEMPLATE)->TEMPLATE$TEMPLATE$TPL_START", steps[2].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE$TPL_START-(TEMPLATE$TEMPLATE$TPL_START_CONN)->TEMPLATE$TEMPLATE$START", steps[3].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE$START-(TEMPLATE$TEMPLATE$START_TEMPLATE)->TEMPLATE$TEMPLATE$TEMPLATE", steps[4].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE$TEMPLATE-(TEMPLATE$TEMPLATE$TEMPLATE_FINISH)->TEMPLATE$TEMPLATE$FINISH", steps[5].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE$FINISH-(TEMPLATE$TEMPLATE$TPL_END_CONN)->TEMPLATE$TEMPLATE$TPL_END", steps[6].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE$TPL_END-(TEMPLATE$TEMPLATE_FINISH)->TEMPLATE$FINISH", steps[7].ToString());
//            Assert.AreEqual("TEMPLATE$FINISH-(TEMPLATE$TPL_END_CONN)->TEMPLATE$TPL_END", steps[8].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_END-(TEMPLATE_FINISH)->FINISH", steps[9].ToString());
//        }

//        [Test]
//        public void ProcessRun_TemplateWithTwoEndActivities()
//        {
//            var process = this.LoadSimpleProcess("Template4");
//            var steps = new List<Transition>();
//            var policy = new TestPolicy(steps);

//            foreach (var activity in process.GetAllActivities())
//            {
//                foreach (var connector in activity.ConnectorsOutgoing)
//                {
//                    connector.DesingerPolicies.Add(new DesingerPolicyItem { Policy = policy });
//                }
//            }

//            WFProcessInstance instance = new WFProcessInstance(process);
//            instance.Box = new WFProcessInstance.TransparentBox(instance);

//            instance = process.Run(instance, new Dictionary<string, object>());
//            instance.ExecuteWorkListSynchronously();

//            Assert.AreEqual(8, steps.Count);
//            Assert.AreEqual("START-(START_TEMPLATE)->TEMPLATE$TPL_START", steps[0].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_START-(TEMPLATE$TPL_START_CONN)->TEMPLATE$START", steps[1].ToString());
//            Assert.AreEqual("TEMPLATE$START-(TEMPLATE$START_TEMPLATE)->TEMPLATE$TEMPLATE", steps[2].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE-(TEMPLATE$TEMPLATE_FINISH1)->TEMPLATE$FINISH1", steps[3].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE-(TEMPLATE$TEMPLATE_FINISH2)->TEMPLATE$FINISH2", steps[4].ToString());
//            Assert.AreEqual("TEMPLATE$FINISH1-(TEMPLATE$TPL_END_CONN)->TEMPLATE$TPL_END", steps[5].ToString());
//            Assert.AreEqual("TEMPLATE$FINISH2-(TEMPLATE$TPL_END_CONN)->TEMPLATE$TPL_END", steps[6].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_END-(TEMPLATE_FINISH)->FINISH", steps[7].ToString());
//        }

//        [Test]
//        public void SubProcessRun_Template1()
//        {
//            var steps = new List<Transition>();
//            var process = WFProcessFactory.GetProcess("Template1", "Subprocess");
//            var policy = new TestPolicy(steps);

//            foreach (var activity in process.GetAllActivities())
//            {
//                foreach (var connector in activity.ConnectorsOutgoing)
//                {
//                    connector.DesingerPolicies.Add(new DesingerPolicyItem { Policy = policy });
//                }
//            }

//            WFProcessInstance instance = new WFProcessInstance(process);
//            instance.Box = new WFProcessInstance.TransparentBox(instance);

//            instance = process.Run(instance, new Dictionary<string, object>());
//            instance.ExecuteWorkListSynchronously();

//            Assert.AreEqual(2, steps.Count);
//            Assert.AreEqual("START-(START_TEMPLATE)->TEMPLATE", steps[0].ToString());
//            Assert.AreEqual("TEMPLATE-(TEMPLATE_FINISH)->FINISH", steps[1].ToString());
//        }

//        [Test]
//        public void SubProcessRun_Template2()
//        {
//            var steps = new List<Transition>();
//            var process = WFProcessFactory.GetProcess("Template2", "Subprocess");
//            var policy = new TestPolicy(steps);

//            foreach (var activity in process.GetAllActivities())
//            {
//                foreach (var connector in activity.ConnectorsOutgoing)
//                {
//                    connector.DesingerPolicies.Add(new DesingerPolicyItem { Policy = policy });
//                }
//            }

//            WFProcessInstance instance = new WFProcessInstance(process);
//            instance.Box = new WFProcessInstance.TransparentBox(instance);

//            instance = process.Run(instance, new Dictionary<string, object>());
//            instance.ExecuteWorkListSynchronously();

//            Assert.AreEqual(6, steps.Count);
//            Assert.AreEqual("START-(START_TEMPLATE)->TEMPLATE$TPL_START", steps[0].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_START-(TEMPLATE$TPL_START_CONN)->TEMPLATE$START", steps[1].ToString());
//            Assert.AreEqual("TEMPLATE$START-(TEMPLATE$START_TEMPLATE)->TEMPLATE$TEMPLATE", steps[2].ToString());
//            Assert.AreEqual("TEMPLATE$TEMPLATE-(TEMPLATE$TEMPLATE_FINISH)->TEMPLATE$FINISH", steps[3].ToString());
//            Assert.AreEqual("TEMPLATE$FINISH-(TEMPLATE$TPL_END_CONN)->TEMPLATE$TPL_END", steps[4].ToString());
//            Assert.AreEqual("TEMPLATE$TPL_END-(TEMPLATE_FINISH)->FINISH", steps[5].ToString());
//        }

//        [Test]
//        public void SubProcessRun_Template3()
//        {
//            var steps = new List<Transition>();
//            var process = WFProcessFactory.GetProcess("Template3", "Subprocess");
//            var policy = new TestPolicy(steps);

//            foreach (var activity in process.GetAllActivities())
//            {
//                foreach (var connector in activity.ConnectorsOutgoing)
//                {
//                    connector.DesingerPolicies.Add(new DesingerPolicyItem { Policy = policy });
//                }
//            }

//            WFProcessInstance instance = new WFProcessInstance(process);
//            instance.Box = new WFProcessInstance.TransparentBox(instance);

//            instance = process.Run(instance, new Dictionary<string, object>());
//            instance.ExecuteWorkListSynchronously();

//            Assert.AreEqual(3, steps.Count);
//            Assert.AreEqual("START-(START_TEMPLATE)->TEMPLATE", steps[0].ToString());
//            Assert.AreEqual("TEMPLATE-(TEMPLATE_FINISH1)->FINISH", steps[1].ToString());
//            Assert.AreEqual("TEMPLATE-(TEMPLATE_FINISH2)->FINISH", steps[2].ToString());
//        }

//        [Test]
//        public void SubProcessRun_Template4()
//        {
//            var steps = new List<Transition>();
//            var process = WFProcessFactory.GetProcess("Template4", "Subprocess");
//            var policy = new TestPolicy(steps);

//            foreach (var activity in process.GetAllActivities())
//            {
//                foreach (var connector in activity.ConnectorsOutgoing)
//                {
//                    connector.DesingerPolicies.Add(new DesingerPolicyItem { Policy = policy });
//                }
//            }

//            WFProcessInstance instance = new WFProcessInstance(process);
//            instance.Box = new WFProcessInstance.TransparentBox(instance);

//            instance = process.Run(instance, new Dictionary<string, object>());
//            instance.ExecuteWorkListSynchronously();

//            Assert.AreEqual(3, steps.Count);
//            Assert.AreEqual("START-(START_TEMPLATE)->TEMPLATE", steps[0].ToString());
//            Assert.AreEqual("TEMPLATE-(TEMPLATE_FINISH1)->FINISH1", steps[1].ToString());
//            Assert.AreEqual("TEMPLATE-(TEMPLATE_FINISH2)->FINISH2", steps[2].ToString());
//        }

//        private WFProcess LoadSimpleProcess(string template)
//        {
//            const string xml = @"
//                <Template Name=""Process"" Version=""1"">
//                  <Activities>
//                    <Activity Code=""START"" Caption=""START"" Description=""START"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""TEMPLATE"" Caption=""Template"" Description=""Template"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFTemplateActivity"" TemplateName=""{0}"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""FINISH"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                  </Activities>
//                  <Connectors>
//                    <Connector From=""START"" To=""TEMPLATE"" Code=""START_TEMPLATE"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH"" Code=""TEMPLATE_FINISH"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                  </Connectors>
//                </Template>
//            ";

//            return ProcessTemplate.LoadFromXml(string.Format(xml, template));
//        }

//        private WFProcess LoadSimpleProcessWithTwoTemplates(string template1, string template2)
//        {
//            const string xml = @"
//                <Template Name=""Process"" Version=""1"">
//                  <Activities>
//                    <Activity Code=""START"" Caption=""START"" Description=""START"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""TEMPLATE1"" Caption=""Template1"" Description="""" Type=""SoftwareMind.SimpleWorkflow.Activities.WFTemplateActivity"" TemplateName=""{0}"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""TEMPLATE2"" Caption=""Template2"" Description="""" Type=""SoftwareMind.SimpleWorkflow.Activities.WFTemplateActivity"" TemplateName=""{1}"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""FINISH"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                  </Activities>
//                  <Connectors>
//                    <Connector From=""START"" To=""TEMPLATE1"" Code=""START_TEMPLATE1"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE1"" To=""TEMPLATE2"" Code=""TEMPLATE1_TEMPLATE2"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE2"" To=""FINISH"" Code=""TEMPLATE2_FINISH"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                  </Connectors>
//                </Template>
//            ";

//            return ProcessTemplate.LoadFromXml(string.Format(xml, template1, template2));
//        }

//        private WFProcess LoadSimpleProcessWithTwoOutgoingConnectors(string template)
//        {
//            const string xml = @"
//                <Template Name=""Process"" Version=""1"">
//                  <Activities>
//                    <Activity Code=""START"" Caption=""START"" Description=""START"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""TEMPLATE"" Caption=""Template"" Description=""Template"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFTemplateActivity"" TemplateName=""{0}"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""FINISH"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                  </Activities>
//                  <Connectors>
//                    <Connector From=""START"" To=""TEMPLATE"" Code=""START_TEMPLATE"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH"" Code=""TEMPLATE_FINISH1"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH"" Code=""TEMPLATE_FINISH2"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                  </Connectors>
//                </Template>
//            ";

//            return ProcessTemplate.LoadFromXml(string.Format(xml, template));
//        }

//        private WFProcess LoadSimpleProcessWithTwoIncomingConnectors(string template)
//        {
//            const string xml = @"
//                <Template Name=""Process"" Version=""1"">
//                  <Activities>
//                    <Activity Code=""START"" Caption=""START"" Description=""START"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""TEMPLATE"" Caption=""Template"" Description=""Template"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFTemplateActivity"" TemplateName=""{0}"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""FINISH"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                  </Activities>
//                  <Connectors>
//                    <Connector From=""START"" To=""TEMPLATE"" Code=""START_TEMPLATE1"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""START"" To=""TEMPLATE"" Code=""START_TEMPLATE2"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH"" Code=""TEMPLATE_FINISH"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                  </Connectors>
//                </Template>
//            ";

//            return ProcessTemplate.LoadFromXml(string.Format(xml, template));
//        }

//        private void PreloadTemplates()
//        {
//            this.PreloadTemplate1();
//            this.PreloadTemplate2();
//            this.PreloadTemplate3();
//            this.PreloadTemplate4();
//        }

//        private void PreloadTemplate1()
//        {
//            const string xml = @"
//                <Template Name=""Template1"" Version=""1"">
//                  <Activities>
//                    <Activity Code=""START"" Caption=""START"" Description=""START"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""TEMPLATE"" Caption=""Template"" Description=""Template"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFScriptActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" Script=""@1==1"" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""FINISH"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                  </Activities>
//                  <Connectors>
//                    <Connector From=""START"" To=""TEMPLATE"" Code=""START_TEMPLATE"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""true"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH"" Code=""TEMPLATE_FINISH"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""true"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                  </Connectors>
//                </Template>
//            ";

//            var template = WFProcessFactory.GetProcessFromXml(xml, "Subprocess");
//        }

//        private void PreloadTemplate2()
//        {
//            const string xml = @"
//                <Template Name=""Template2"" Version=""1"">
//                  <Activities>
//                    <Activity Code=""START"" Caption=""START"" Description=""START"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""TEMPLATE"" Caption=""Template"" Description=""Template"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFTemplateActivity"" TemplateName=""Template1"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""FINISH"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                  </Activities>
//                  <Connectors>
//                    <Connector From=""START"" To=""TEMPLATE"" Code=""START_TEMPLATE"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH"" Code=""TEMPLATE_FINISH"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                  </Connectors>
//                </Template>
//            ";

//            var template = WFProcessFactory.GetProcessFromXml(xml, "Subprocess");
//        }

//        private void PreloadTemplate3()
//        {
//            const string xml = @"
//                <Template Name=""Template3"" Version=""1"">
//                  <Activities>
//                    <Activity Code=""START"" Caption=""START"" Description=""START"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""TEMPLATE"" Caption=""Template"" Description=""Template"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFScriptActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" Script=""@1=1"" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""FINISH"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                  </Activities>
//                  <Connectors>
//                    <Connector From=""START"" To=""TEMPLATE"" Code=""START_TEMPLATE"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH"" Code=""TEMPLATE_FINISH1"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH"" Code=""TEMPLATE_FINISH2"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                  </Connectors>
//                </Template>
//            ";

//            var template = WFProcessFactory.GetProcessFromXml(xml, "Subprocess");
//        }

//        private void PreloadTemplate4()
//        {
//            const string xml = @"
//                <Template Name=""Template4"" Version=""1"">
//                  <Activities>
//                    <Activity Code=""START"" Caption=""START"" Description=""START"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFStartActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""TEMPLATE"" Caption=""Template"" Description=""Template"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFScriptActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" Script=""@1=1"" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""FINISH1"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                    <Activity Code=""FINISH2"" Caption=""FINISH"" Description=""FINISH"" Type=""SoftwareMind.SimpleWorkflow.Activities.WFEndActivity"" DesignerSettings=""0,0,0,0"" StartScript="""" EndScript="""" ExecuteScript="""" LongRunning=""false"">
//                      <MetaDataCollection />
//                    </Activity>
//                  </Activities>
//                  <Connectors>
//                    <Connector From=""START"" To=""TEMPLATE"" Code=""START_TEMPLATE"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH1"" Code=""TEMPLATE_FINISH1"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                    <Connector From=""TEMPLATE"" To=""FINISH2"" Code=""TEMPLATE_FINISH2"" DialogBoxName="""" SingleInstance=""False"" IsSystem=""False"" IsActiveWithoutTodos=""False"" Caption="""" DesignerSettings=""0;0;0;0;0;0;0;0"" ConditionScript="""" PolicyScript="""" ConnectorBehaviour=""SoftwareMind.SimpleWorkflow.Behaviours.WFStandardConnectorBehaviour, SoftwareMind.SimpleWorkflow"" DestinationSplitter="""" MainPath=""false"" Order=""10"" DeleteParentPath=""0"" ShowAsButton=""False"">
//                      <Conditions />
//                      <Policies />
//                      <MetaDataCollection />
//                      <DialogBoxValueMaping />
//                      <DialogBoxParameters />
//                    </Connector>
//                  </Connectors>
//                </Template>
//            ";

//            var template = WFProcessFactory.GetProcessFromXml(xml, "Subprocess");
//        }
//    }

//    struct Transition
//    {
//        public string From;
//        public string To;
//        public string Connector;

//        public Transition(string @from, string to, string connector)
//        {
//            From = @from;
//            To = to;
//            Connector = connector;
//        }

//        public override string ToString()
//        {
//            return string.Format("{0}-({2})->{1}", this.From, this.To, this.Connector);
//        }
//    }

//    class TestPolicy : IWFPolicyRule
//    {
//        private readonly IList<Transition> _steps;

//        public TestPolicy(IList<Transition> steps)
//        {
//            this._steps = steps;
//        }

//        public void ReadTemplateFromXmlElement(XElement element)
//        {
//            throw new NotImplementedException();
//        }

//        public XElement WriteTemplateToXmlElement()
//        {
//            throw new NotImplementedException();
//        }

//        public void Check(IWFProcessInstance processInstance, IDictionary<string, object> arguments)
//        {
//            this._steps.Add(new Transition(processInstance.TransitionConnector.Source.Code, processInstance.TransitionConnector.Destination.Code, processInstance.TransitionConnector.Code));
//        }
//    }
//}
