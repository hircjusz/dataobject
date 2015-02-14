using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using log4net;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    /// <summary>
    /// Zwielokrotnia zadanie. Dla podanego zadanie może zwrócić liczbe zadań które różnią się
    /// jedynie zmiennymi wejściowymi, których wygenerowaniem zajmuje sie ta klasa
    /// </summary>
    [Serializable]
    public class ListSplitter : IWFWorkSplitter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ListSplitter));

        public string ListVariableName { get; set; }

        public ListSplitter()
        {
            log.Debug("ListSplitter. Zwielokrotnianie zadania");
        }

        public IDictionary<string, object>[] GetInputData(IWFActivityInstance activityInstance)
        {
            List<IDictionary<string, object>> inputData = new List<IDictionary<string, object>>();

            if (string.IsNullOrWhiteSpace(ListVariableName))
                throw new InvalidOperationException("Nazwa zmiennej zawierającej kolekcje nie została podana");

            if (activityInstance == null)
                throw new ArgumentException("Aktywność, która ma być zwielokrotniona, nie została podana");

            if (activityInstance.GetDefContainer().GetVariable(ListVariableName).IsCollection == false)
                throw new ArgumentException("Podany parametr nie jest kolekcją");

            object[] variables = ((IEnumerable<object>)activityInstance.ProcessInstance.GetVariableValue(ListVariableName)).ToArray();

            foreach(var obj in variables)
            {
                IDictionary<string, object> input = new Dictionary<string, object>();
                input.Add(ListVariableName, obj);
                inputData.Add(input);
            }

            if (inputData.Count == 0) throw new ArgumentException("Podany parametr nie zawiera danych do podziału");

            return inputData.ToArray();
        }

        public void ReadTemplateFromXmlElement(XElement element)
        {
            this.ListVariableName = element.Attribute("ListVariableName").Value;
        }

        public XElement WriteTemplateToXmlElement()
        {
            return new XElement("IWFWorkSplitter",
                new XAttribute("Type", this.GetType()),
                new XAttribute("ListVariableName", this.ListVariableName??"")
                );
        }
    }
}
