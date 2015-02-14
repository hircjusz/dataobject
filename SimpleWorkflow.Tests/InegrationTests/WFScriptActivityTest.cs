using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SoftwareMind.SimpleWorkflow.Activities;

namespace SoftwareMind.SimpleWorkflow.Tests.InegrationTests
{
    [TestFixture]
    public class WFScriptActivityTest
    {
        [Test, Category("integration")]
        public void ExecutingSimpleScript()
        {
            string source = @"result = a + b;";

            WFActivityBase scriptActivity = new WFScriptActivity()
            {
                Script = source,
                Code = "SCRIPT",
            };

            MockRepository mocks = new MockRepository();
            IWFProcessInstance processInstance = mocks.Stub<IWFProcessInstance>();
            using (mocks.Record())
            {
                processInstance.GetVariables();
                LastCall.Return(new Dictionary<string, object>());
            }


            var env = new Dictionary<string, object>(){{"a", 1}, {"b", 2}};
            TestActivityInstance tai = new TestActivityInstance();
            tai.Variables = env;
            tai.State = WFActivityState.Initialized;
            tai.ProcessInstance = processInstance;
            scriptActivity.ChangeState(tai/*, null*/);

            Assert.AreEqual(env["result"], 3);
        }


    }

    #region stubs

    [Serializable]
    public class TestActivityInstance : IWFActivityInstance
    {

        public WFActivityBase Activity
        {
            get { throw new NotImplementedException(); }
        }

        public string Code
        {
            get
            {
                return "CODE";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IWFActivityInstance GetOrCreateActivity(string activityCode, Type activityType)
        {
            throw new NotImplementedException();
        }


        WFActivityState state;
        public WFActivityState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public IEnumerable<WFConnector> GetAvailableTransitions(IDictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public bool IsCompleted()
        {
            throw new NotImplementedException();
        }
        public bool Complete(IDictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public bool HasParent()
        {
            throw new NotImplementedException();
        }

        public string GetParentPath()
        {
            throw new NotImplementedException();
        }

        public void AddVariable(WFVariable variable)
        {
            throw new NotImplementedException();
        }

        public void AddVariableValue(string name, object value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsVariable(string name)
        {
            return Variables.ContainsKey(name);
        }

        public WFVariableDefContainer GetDefContainer()
        {
            throw new NotImplementedException();

        }

        public WFVariable GetVariable(string name)
        {
            throw new NotImplementedException();
        }

        public object GetVariableValue(string name)
        {
            return Variables[name];
        }

        public void AddVariables(IDictionary<string, object> activityArguments)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> Variables { get; set; }

        public IDictionary<string, object> GetVariables()
        {
            return Variables;
        }

        public void ReadStateFromXmlElement(System.Xml.Linq.XElement element)
        {
            throw new NotImplementedException();
        }

        public System.Xml.Linq.XElement WriteStateToXmlElement()
        {
            throw new NotImplementedException();
        }


        public void SetVariableValue(string name, object value)
        {
            Variables[name] = value;
        }


        public object GetVariableValueOrDefault(string name)
        {
            throw new NotImplementedException();
        }

        #region IWFActivityInstance Members

        IWFProcessInstance _processInstance;

        public IWFProcessInstance ProcessInstance
        {
            get
            {
                return _processInstance;
            }
            set
            {
                _processInstance = value;
            }
        }

        public IWFProcessInstanceBox Box
        {
            get
            {
                return ProcessInstance.Box;
            }
        }

        #endregion

        public void Abort(IDictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }


        #region IWFActivityInstance Members


        IEnumerable<IWFConnector> IWFActivityInstance.GetAvailableTransitions(IDictionary<string, object> args)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IWFActivityInstance Members


        public WFTransitionStatus DoTransition(IWFActivityInstance activityInstance, string connectorCode, Dictionary<string, object> arguments = null)
        {
            throw new NotImplementedException();
        }

        #endregion


        public void RemoveVariable(string name)
        {
            throw new NotImplementedException();
        }


        public WFTransitionStatus DoTransition(IWFActivityInstance activityInstance, string connectorCode, Action<IWFActivityInstance> beforeCompleted = null, Dictionary<string, object> arguments = null)
        {
            throw new NotImplementedException();
        }


        public void ManualChangeState(string stepCode, WFActivityState state)
        {
            throw new NotImplementedException();
        }

        #region IWFActivityInstance Members


        public IEnumerable<IWFConnectorBase> GetTransitions(IDictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        #endregion


        public void ManualChangeState(string stepCode, WFActivityState state, IWFProcessInstance process, Action<IWFProcessInstance> action)
        {
            throw new NotImplementedException();
        }

        #region IWFActivityInstance Members


        public IWFConnectorBase GetDefaultTransition(IDictionary<string, object> args)
        {
            throw new NotImplementedException();
        }

        #endregion


        public void TryGetValue(string name, TryGetValueOperation operation)
        {
            throw new NotImplementedException();
        }


        public string GetAncestorPath()
        {
            throw new NotImplementedException();
        }


        public string GetPrefixFromParams(params string[] codes)
        {
            throw new NotImplementedException();
        }


        public string GetVariableNameFromSubprocess(string varName)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
