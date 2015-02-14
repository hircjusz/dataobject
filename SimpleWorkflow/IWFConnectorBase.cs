using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Behaviours;

namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFConnectorBase
    {
        string Caption { get; set; }
        string Code { get; set; }
        bool SingleInstance { get;  }
        string DialogBoxName { get;  }
        bool IsSystem{ get; }
        bool IsConnectorAvailable { get; }
        bool IsActiveWithoutTodos { get; }
        WFActivityBase Destination { get; }
        EditableList<MappingItem> DialogBoxValueMaping {  get; }
        EditableList<ParameterItem> DialogBoxParameters { get; }
        EditableList<WFActivityMetaData> MetaData { get; }
        WFConnectorBehaviourType ConnectorBehaviourType { get; }
        int Order { get; set; }
        bool ShowAsButton { get; set; }
        Type DestinationSplitterType { get; }
    }
}
