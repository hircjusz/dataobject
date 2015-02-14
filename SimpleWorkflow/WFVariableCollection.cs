using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoftwareMind.Scripts;

namespace SoftwareMind.SimpleWorkflow
{
    /// <summary>
    /// Klasa reprezentująca kolekcję zmiennych. Opakowywuje słownik, by łatwo można było przekazać ją do skryptów jako kolekcję
    /// nazwa-wartość.
    /// </summary>
    [Serializable]
    public class WFVariableCollection : CollectionBase, IDictionary<string, object>
    {

        private Dictionary<string, WFVariable> dic = new Dictionary<string, WFVariable>();

        public WFVariableCollection()
        {
        }

        public void AddRange(WFVariableCollection source)
        {
            foreach (var item in source.dic)
            {
                WFVariable variable;
                if (dic.TryGetValue(item.Key, out variable))
                {
                    variable.Type = item.Value.Type;
                    variable.Value = item.Value.Type;
                    variable.IsCollection = item.Value.IsCollection;
                    variable.Direction = item.Value.Direction;
                }
                else
                    dic[item.Key] = item.Value;
            }
        }

        public new int Count
        {
            get
            {
                return dic.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ICollection<string> Keys
        {
            get { return dic.Keys; }
        }

        public WFVariable this[string name]
        {
            get
            {
                return dic[name];
            }
            set
            {
                if (name != value.Name)
                    throw new ArgumentException("nazwa zmiennej i indeksowan nazwa nie mogą byc różne.");
                if (dic.ContainsKey(name))
                {
                    Remove(value);
                    Add(value);
                }
                else
                    Add(value);
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return dic.Values.Select(x => x.Value).ToList();
            }
        }

        public void Add(WFVariable variable)
        {
            if (variable.Collection != null)
                if (variable.Collection == this)

                    return;
                else
                    throw new ArgumentException(String.Format("Zmienna {0} należy już do innej kolekcji.", variable.Name));

            ValidateVariable(variable);
            if (dic.ContainsKey(variable.Name))
                throw new ArgumentException("Nazwa zmiennej jest już zajęta.");

            dic[variable.Name] = variable;
            variable.Collection = this;
        }

        public void Add(string key, object value)
        {
            Add(new WFVariable() { Name = key, Value = value });
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(WFVariable variable)
        {
            return dic.ContainsKey(variable.Name);
        }

        public bool Contains(string name)
        {
            return dic.ContainsKey(name);
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return dic.ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return Contains(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new InvalidOperationException();
        }

        public new IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var item in dic)
                yield return new KeyValuePair<string, object>(item.Key, item.Value.Value);
        }

        public IDictionary<string, Type> GetTypeDictionary()
        {
            return dic.ToDictionary(x => x.Key, x =>
                {
                    if (x.Value.Value != null)
                        return x.Value.Value.GetType();
                    else if (x.Value.Type != null)
                    {
                        if (!x.Value.IsCollection)
                            return x.Value.Type;
                        else
                        {
                            var type = typeof(ICollection<>);
                            return type.MakeGenericType(x.Value.Type);
                        }
                    }
                    else
                        return typeof(DynamicScriptObject);
                });
        }

        public void Remove(WFVariable variable)
        {
            dic.Remove(variable.Name);
            variable.Collection = null;
        }

        public bool Remove(string key)
        {
            var variable = dic[key];
            bool result = dic.Remove(key);
            variable.Collection = null;
            return result;
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(string key, out object value)
        {
            WFVariable v;
            bool ret = dic.TryGetValue(key, out v);
            value = v;
            return ret;
        }

        protected override void OnClearComplete()
        {
        }

        protected override void OnInsertComplete(int index, object value)
        {
            Add(value as WFVariable);
        }



        protected override void OnRemoveComplete(int index, object value)
        {
            WFVariable variable = (WFVariable)value;
            if(dic.ContainsKey(variable.Name))
            Remove(variable.Name);

        }


        private void ValidateVariable(object value)
        {
            WFVariable v = (WFVariable)value;
            if (String.IsNullOrEmpty(v.Name))
                throw new ArgumentException("Nazwa zmiennej nie mozę być pusta.");
        }


        object IDictionary<string, object>.this[string key]
        {
            get
            {
                return dic[key].Value;
            }
            set
            {
                if (dic.ContainsKey(key))
                    dic[key].Value = value;
                else
                    Add(key, value);
            }
        }
    }
}
