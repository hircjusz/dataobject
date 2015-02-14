using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using SoftwareMind.SimpleWorkflow.Actions;
using SoftwareMind.SimpleWorkflow.Activities;

namespace SoftwareMind.SimpleWorkflow.Tests.InegrationTests
{
    [TestFixture]
    public class WfActivityActionsTest
    {
        [Test, Category("integration")]
        public void ActionInit()
        {
            var activity = new WFManualActivity()
            {
                Code = "TEST"
            };
            Assert.IsNotNull(activity.ActivateTaskActions);
            Assert.IsNotNull(activity.UnassignOwnerActions);
            Assert.IsNotNull(activity.AssignOwnerActions);
            Assert.IsNotNull(activity.CompleteTaskActions);
        }

        [Test, Category("integration")]
        public void CreateDefaultParam()
        {
            var actionContainer = new WfActivityActionContainer();

            actionContainer.Action = typeof (TestAction);
            Assert.IsNotNull(actionContainer.Parameters);
            Assert.IsInstanceOf<TestActionParameters>(actionContainer.Parameters);

            actionContainer.Action = null;
            Assert.IsNull(actionContainer.Parameters);
        }

        [Test, Category("integration")]
        public void CreateAction()
        {
            var actionContainer = new WfActivityActionContainer();

            actionContainer.Action = typeof(TestAction);
            var action = actionContainer.CreateAction();
            Assert.IsNotNull(action);
            Assert.IsInstanceOf<TestAction>(action);
        }

        #region Mock classes

        private class TestAction : IWfActivityAction
        {
            public void Perform(IWfActivityActionParameters parameter, Dictionary<string, object> context)
            {
                throw new NotImplementedException();
            }

            public IWfActivityActionParameters CreateDefaultParameter()
            {
                return new TestActionParameters();
            }
        }

        private class TestActionParameters : IWfActivityActionParameters
        {
            public object Clone()
            {
                throw new NotImplementedException();
            }

            public void ReadTemplateFromXmlElement(XElement element)
            {
                throw new NotImplementedException();
            }

            public XElement WriteTemplateToXmlElement()
            {
                throw new NotImplementedException();
            }

            public void Validate()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
