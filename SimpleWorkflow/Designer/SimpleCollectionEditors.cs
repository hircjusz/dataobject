using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using SoftwareMind.SimpleWorkflow.Actions;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    public class WfActivityActionContainerCollectionEditor : CollectionEditorWithName
    {
        private static string GetName(object value)
        {
            var actionContainer = value as WfActivityActionContainer;
            if (actionContainer == null)
            {
                return null;
            }
            return actionContainer.Name;
        }

        public WfActivityActionContainerCollectionEditor(Type type)
            : base(GetName, type)
        {
        }
    }

    public abstract class CollectionEditorWithName : CollectionEditor
    {
        private Func<object, string> getNameHandler;

        protected CollectionEditorWithName(Func<object, string> getNameHandler, Type type) : base(type)
        {
            this.getNameHandler = getNameHandler;
        }

        protected override string GetDisplayText(object value)
        {
            if (getNameHandler == null)
            {
                return base.GetDisplayText(value);
            }
            return base.GetDisplayText(getNameHandler(value));
        }
    }
}
