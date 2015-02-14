using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Misc;
using log4net;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa pozwalajaca na mapowanie parametrów zminnych na parametry metod
    /// </summary>
    [Serializable]
    public class VariableMapper : IWFTemplateElement
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(VariableMapper));

        /// <summary>
        /// Metoda do mapowania
        /// </summary>
        private MethodBase method;
        /// <summary>
        /// Mapowanie
        /// </summary>
        private string[] map;



        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="VariableMapper"/>.
        /// </summary>
        public VariableMapper()
        {
        }



        /// <summary>
        /// Pozwala pobrać i ustawić metodę do mapowania
        /// </summary>
        /// <value>Metoda</value>
        public MethodBase Method
        {
            get
            {
                return method;
            }
            set
            {
                if (value != null)
                {
                    if (method != value)
                    {
                        List<string> newMap = new List<string>();
                        foreach (var parameter in value.GetParameters())
                            newMap.Add(string.Format("{0}=", parameter.Name));
                        map = newMap.ToArray();
                    }
                }
                else
                    map = null;
                method = value;
            }
        }

        /// <summary>
        /// Mapowanie
        /// </summary>
        /// <value>Mapowanie</value>
        public String[] VariableMap
        {
            get
            {
                return map;
            }
            set
            {
                map = value;
            }
        }




        /// <summary>
        /// Mapuje ścieżki na wartosci zmiennych
        /// </summary>
        /// <param name="variables">Kolekcja zmiennych.</param>
        /// <param name="defs">Definicje zmiennych</param>
        /// <returns>Zmapowane wartości.</returns>
        public object[] Map(IDictionary<string, object> variables, WFVariableDefContainer defs)
        {
            if (Method == null)
                throw new InvalidOperationException("Przed wykonaniem mapowania należy wskazać metodę");

            ParameterInfo[] parameters = Method.GetParameters();
            Dictionary<string, object> mapedParameters = new Dictionary<string, object>();
            string[][] entries = map.Select(x=> new {entry = x, splitIndex = x.IndexOf('=')}).
                Select(x=> new string[] { x.entry.Remove(x.splitIndex), x.entry.Substring(1 + x.splitIndex) }).ToArray();

            entries = entries.OrderBy(x => x[0].Count(y => y == '.')).ThenBy(x => x[0]).ToArray();
            string[] parameterNames = parameters.Select(x => x.Name).ToArray();
            string[] simpleEntries = entries.Where(x=> !x[0].Contains('.')).Select(x=>x[0]).ToArray();
            string[] unmapedParamters = parameterNames.Where(x => !simpleEntries.Contains(x)).ToArray();

            for (int i = 0; i < entries.Length; i++)
            {
                string[] entry = entries[i];
                try
                {
                    object value = null;
                    string[] left = entry[0].Split('.');
                    bool ignore = false;
                    string[] right = entry[1].Split('.');
                    object variable = variables.ContainsKey(right[0]) ? variables[right[0]] : defs.VariableDefs[right[0]].GetDefaultValue();
                    GetRightPathValue(variable, right, ref value, left, ref ignore);
                    if (!ignore)
                        if (left.Length == 1)
                            mapedParameters[entry[0]] = value;
                        else
                        {
                            if (!mapedParameters.ContainsKey(left[0]))
                                mapedParameters[left[0]] = Activator.CreateInstance(parameters.Single(x => x.Name == left[0]).ParameterType);
                            SetValue(mapedParameters[left[0]], left, 1, value);
                        }
                }
                catch (Exception ex)
                {
                    log.Error("Mapowanie wpsiu " + entries[i][0] + " = " + entries[i][1] + " nie powiodło się.", ex);
                    throw new InvalidOperationException("Mapowanie wpsiu " + entries[i][0] + " = " + entries[i][1] + " nie powiodło się.", ex);
                }
            }
            foreach (var item in unmapedParamters)
                if(!mapedParameters.ContainsKey(item))
                    mapedParameters[item] = null;

            return parameters.Select(x => mapedParameters[x.Name]).ToArray();
        }

        /// <summary>
        /// Ustawia wartości argumentów przekazanych do funkcji z modyfikatorem ref i out.
        /// </summary>
        /// <param name="variables">Zmienne.</param>
        /// <param name="args">Argumenty.</param>
        public void MapBack(IDictionary<string, object> variables, object[] args)
        {
            foreach (var parameter in Method.GetParameters().Where(x => x.IsOut || x.ParameterType.IsByRef))
            {
                string entry = map.Where(x => x.StartsWith(parameter.Name + "=")).FirstOrDefault();
                if (entry != null)
                {
                    string[] right = entry.Substring(entry.IndexOf('=') + 1).Split('.');
                    if (right.Length == 0)
                        continue;
                    else if (right.Length == 1)
                        variables[right[0]] = args[parameter.Position];
                    else
                        SetValue(variables[right[0]], right, 1, args[parameter.Position]);
                }
            }
        }

        /// <summary>
        /// Wczytuje obiekt z xmlaa
        /// </summary>
        /// <param name="element">Węzeł xmla.</param>
        public void ReadTemplateFromXmlElement(XElement element)
        {
            element = element.Descendants("Map").First();
            map = SplitParameterMap(element.Value);
        }

        /// <summary>
        /// Zwraca tekstową reprezentacje klasy.
        /// </summary>
        /// <returns>
        /// String.
        /// </returns>
        public override string ToString()
        {
            if (VariableMap == null)
                return "";
            if (VariableMap.Length == 0)
                return string.Empty;
            else
                return string.Join(";", VariableMap);
        }

        /// <summary>
        /// Zapisuje węzeł do xmla.
        /// </summary>
        /// <returns></returns>
        public XElement WriteTemplateToXmlElement()
        {
            var element = new XElement("Map", map.Length != 0 ? String.Join(";", map) : "");
            return element;
        }

        /// <summary>
        /// Zwraca wartosć getera
        /// </summary>
        /// <param name="variable">Zmienna.</param>
        /// <param name="path">ścieżka.</param>
        /// <param name="actualPropertyIndex">Aktualnie rozpatrywany indeks.</param>
        /// <param name="createNewIfNull"></param>
        /// <returns></returns>
        private static object GetGetPeopretyValue(object variable, string[] path, int actualPropertyIndex, bool createNewIfNull = false)
        {
            Type type = variable.GetType();
            PropertyInfo propInfo = type.GetProperty(path[actualPropertyIndex]);
            if (propInfo != null)
            {
                object result = propInfo.GetGetMethod().Invoke(variable, new object[0]);
                if (result == null && createNewIfNull)
                {
                    result = Activator.CreateInstance(propInfo.PropertyType);
                    propInfo.GetSetMethod().Invoke(variable, new object[] { result });
                }
                return result;
            }
            else
            {
                FieldInfo field = type.GetField(path[actualPropertyIndex]);
                object result = field.GetValue(variable);
                if (result == null && createNewIfNull)
                {
                    result = Activator.CreateInstance(field.FieldType);
                    field.SetValue(variable, result);
                }
                return result;
            }
        }

        /// <summary>
        /// Zwraca wartosć zmiennej
        /// </summary>
        /// <param name="variable">Zmienna.</param>
        /// <param name="right">Cały wpis.</param>
        /// <param name="value">Wartość.</param>
        /// <param name="left">lewa strona ścieżki.</param>
        /// <param name="ignore">Jeśli jest ustawniony na <c>true</c> to ścieżka nie została podana.</param>
        private void GetRightPathValue(object variable, string[] right, ref object value, string[] left, ref bool ignore)
        {
            if (right.Length == 0)
            {
                if (left.Length == 1)
                    value = null;
                else
                    ignore = true;
            }
            else
            {
                value = (right.Length != 0) ? GetValue(variable, right, 1) : null;
            }
        }

        /// <summary>
        /// Zwraca wartość włąściwosci
        /// </summary>
        /// <param name="variable">Zmienna.</param>
        /// <param name="path">ścieżka.</param>
        /// <param name="actualPropertyIndex">Indeks aktualnie rozpatrywanej właściwosći..</param>
        /// <returns>Wartość.</returns>
        private object GetValue(object variable, string[] path, int actualPropertyIndex)
        {
            if (actualPropertyIndex >= path.Length)
                return variable;

            object result = GetGetPeopretyValue(variable, path, actualPropertyIndex, false);
            return GetValue(result, path, actualPropertyIndex + 1);
        }

        /// <summary>
        /// Ustawia wartość właściowści
        /// </summary>
        /// <param name="variable">Zmienna.</param>
        /// <param name="left">Lewa strona mapowania.</param>
        /// <param name="actualPropertyIndex">Aktualnie rozpatrywany indeks.</param>
        /// <param name="value">Wartość.</param>
        private void SetValue(object variable, string[] left, int actualPropertyIndex, object value)
        {
            if (actualPropertyIndex + 1 == left.Length)
            {
                Type type = variable.GetType();
                PropertyInfo propInfo = type.GetProperty(left[actualPropertyIndex]);
                propInfo.GetSetMethod().Invoke(variable, new object[] { value });
            }
            else
            {
                object result = GetGetPeopretyValue(variable, left, actualPropertyIndex, true);
                SetValue(result, left, actualPropertyIndex + 1, value);
            }
        }

        /// <summary>
        /// Tworzy mapowanie z rekurencyjnego wywołanie konstruktora obiektu
        /// </summary>
        /// <param name="entry">Wpis w mapowaniu.</param>
        /// <param name="typeToCreated">Typ do utworzenia.</param>
        /// <param name="constructor">Konstruktor do wywołania.</param>
        /// <param name="rec">Mapowanie zmiennych.</param>
        internal static void GetVarialbleMap(string entry, out string typeToCreated, out MethodBase constructor, out VariableMapper rec)
        {
            typeToCreated = entry.Substring(4);
            typeToCreated = typeToCreated.Remove(typeToCreated.IndexOf('('));
            Type type = TypeHelper.GetType(typeToCreated);
            string selectedConstructor = entry.Substring(entry.IndexOf('(') + 1);
            selectedConstructor = selectedConstructor.Remove(selectedConstructor.IndexOf(':'));
            constructor = MethodBaseHelper.GetMethodBase(type, selectedConstructor);

            string parametersString = entry.Substring(entry.IndexOf(':') + 1);
            parametersString = parametersString.Remove(parametersString.IndexOf(')'));

            rec = new VariableMapper();
            rec.Method = constructor;
            rec.VariableMap = SplitParameterMap(parametersString);
            if (rec.VariableMap.Length == 1 && String.IsNullOrEmpty(rec.VariableMap[0]))
                rec.VariableMap = new string[0];
        }

        /// <summary>
        /// Dzieli cały łańcuch mapowania na pojedyńcze zadania do wykonania.
        /// </summary>
        /// <param name="parametersString">łancuch do podzielenia.</param>
        /// <returns>Pojedyńcze mapowania.</returns>
        internal static string[] SplitParameterMap(string parametersString)
        {
            int bracketCounter = 0;
            List<int> splitIndexes = new List<int>();
            splitIndexes.Add(-1);
            for (int i = 0; i < parametersString.Length; i++)
                switch (parametersString[i])
                {
                    case '(':
                        bracketCounter++;
                        break;
                    case ')':
                        bracketCounter--;
                        break;
                    case ';':
                        if (bracketCounter == 0)
                            splitIndexes.Add(i);
                        break;
                }
            splitIndexes.Add(parametersString.Length);
            List<string> result = new List<string>();
            for (int i = 0; i < splitIndexes.Count - 1; i++)
                result.Add(parametersString.Substring(splitIndexes[i] + 1, splitIndexes[i + 1] - splitIndexes[i] - 1));
            return result.ToArray();
        }
    }
}
