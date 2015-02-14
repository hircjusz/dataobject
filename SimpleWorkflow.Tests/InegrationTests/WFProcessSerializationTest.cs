using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SoftwareMind.SimpleWorkflow.Activities;
using SoftwareMind.SimpleWorkflow.Attributes;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Tests.InegrationTests
{
    [TestFixture]
    public class WFProcessSerializationTest
    {
        MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            WFProcessFactory.processes.Clear();
            mocks = null;
        }

        [Test, Category("integration")]
        public void VariableDefSerialization_String()
        {
            WFProcess process = new WFProcess();

            WFStartActivity start = new WFStartActivity() { Code = "START" };
            process.AddActivity(start);
            process.RootActivity = start;

            WFVariableDef definition = new WFVariableDef("session", typeof(string))
            {
                IsCollection = false,
                Direction = WFVariableType.In
            };

            process.VariableDefs.Add(definition);
            string processXml = ProcessTemplate.SaveToXml(process);
            WFProcess afterDeserialization = ProcessTemplate.LoadFromXml(processXml);

            Assert.IsTrue(afterDeserialization.VariableDefs.Contains("session"));
            Assert.IsTrue(afterDeserialization.VariableDefs["session"].Type == typeof(string));
            Assert.IsFalse(afterDeserialization.VariableDefs["session"].IsCollection);
            Assert.AreEqual(WFVariableType.In, afterDeserialization.VariableDefs["session"].Direction);
        }

        [Test, Category("integration")]
        public void VariableDefSerialization_ComplexObject()
        {

            WFProcess process = CreateOneStepProcessWithVariable
                (new WFVariableDef("process", typeof(myDataObjectStub))
                    {
                        IsCollection = false,
                        Direction = WFVariableType.InOut
                    }
                );

            string processXml = ProcessTemplate.SaveToXml(process);
            WFProcess afterDeserialization = ProcessTemplate.LoadFromXml(processXml);

            Assert.IsTrue(afterDeserialization.VariableDefs.Contains("process"));
            Assert.IsTrue(afterDeserialization.VariableDefs["process"].Type == typeof(myDataObjectStub));
            Assert.IsFalse(afterDeserialization.VariableDefs["process"].IsCollection);
            Assert.AreEqual(WFVariableType.InOut, afterDeserialization.VariableDefs["process"].Direction);
        }

        private static WFProcess CreateOneStepProcessWithVariable(WFVariableDef definition)
        {
            WFProcess process = new WFProcess();

            WFStartActivity start = new WFStartActivity() { Code = "START" };
            process.AddActivity(start);
            process.RootActivity = start;
            process.VariableDefs.Add(definition);
            process.Name = "process";
            return process;
        }

        [Test, Category("integration")]
        public void VariableSerialization_ComplexObject()
        {
            WFProcess process = CreateOneStepProcessWithVariable
                (new WFVariableDef("process", typeof(myDataObjectStub))
                {
                    IsCollection = false,
                    Direction = WFVariableType.InOut
                }
            );

            Dictionary<string, object> input = new Dictionary<string,object>();
            input.Add("process", new myDataObjectStub(123));
            WFProcessFactory.processes.Add("process", process);
            IWFProcessInstance procInstance = WFProcessCoordinator.Instance.RunProcess(new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(null), "process", input);
            string state = procInstance.WriteStateToXmlElement().ToString();
            WFProcessInstance newInstance = WFProcessInstance.ReadStateFromXmlString(state);
            Assert.IsTrue(newInstance.ContainsVariable("process"));
        }
    }

    [WFCustomSerialization(typeof(myDataObjectStub))]
    class myDataObjectStub : IWFVariableSerializer
    {
        public int ID { get; set; }

        public myDataObjectStub()
        { }

        public myDataObjectStub(int id)
        {
            this.ID = id;
        }

        public string SerializeValue(object obj)
        {
            return ((myDataObjectStub)obj).ID.ToString();
        }

        public object Deserialize(object value, IDictionary<string, WFVariable> otherVariables)
        {
            myDataObjectStub temp = new myDataObjectStub(int.Parse(value.ToString()));
            return temp;
        }
    }
}
