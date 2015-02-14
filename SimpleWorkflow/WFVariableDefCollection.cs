using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class WFVariableDefCollection : CollectionBase, IDictionary<string, WFVariableDef>
    {

        private Dictionary<string, WFVariableDef> dic = new Dictionary<string, WFVariableDef>();

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

        public WFVariableDef this[string name]
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

        public ICollection<WFVariableDef> Values
        {
            get
            {
                return dic.Values;
            }
        }

        public void Add(WFVariableDef variable)
        {
            if (variable.Collection != null)
                if (variable.Collection == this)
                    return;
                else
                    throw new ArgumentException(String.Format("Zmienna {0} należy już do innej kolekcji.", variable.Name));

            if (dic.ContainsKey(variable.Name))
                throw new ArgumentException("Nazwa zmiennej jest już zajęta.");

            dic[variable.Name] = variable;
            variable.Collection = this;
            WFVariableDef def = List.OfType<WFVariableDef>().Where(x=>x.Name == variable.Name).FirstOrDefault();
            if(def == null)
                List.Add(variable);
        }

        public void Add(string key, WFVariableDef def)
        {
            this.dic.Add(key, def);
        }

        public void Add(KeyValuePair<string, WFVariableDef> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(WFVariableDefCollection items)
        {
            foreach (var item in items)
            {
                if (dic.ContainsKey(item.Key))
                    dic.Remove(item.Key);
                Add(item.Key, item.Value);
            }
        }

        public bool Contains(WFVariableDef variable)
        {
            return dic.ContainsKey(variable.Name);
        }

        public bool Contains(string name)
        {
            return dic.ContainsKey(name);
        }

        public bool Contains(KeyValuePair<string, WFVariableDef> item)
        {
            return dic.ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return Contains(key);
        }

        public void CopyTo(KeyValuePair<string, WFVariableDef>[] array, int arrayIndex)
        {
            throw new InvalidOperationException();
        }

        public new IEnumerator<KeyValuePair<string, WFVariableDef>> GetEnumerator()
        {
            foreach (var item in dic)
                yield return new KeyValuePair<string, WFVariableDef>(item.Key, item.Value);
        }

        public IDictionary<string, Type> GetTypeDictionary()
        {
            return dic.ToDictionary(x => x.Key, x => x.Value.GetDefType());
        }

        public void Remove(WFVariableDef variable)
        {
            dic.Remove(variable.Name);
            variable.Collection = null;
            RemoveFromList(variable);
        }

        public bool Remove(string key)
        {
            var variable = dic[key];
            bool result = dic.Remove(key);
            variable.Collection = null;
            RemoveFromList(variable);
            return result;
        }

        private void RemoveFromList(WFVariableDef variable)
        {
            WFVariableDef def = List.OfType<WFVariableDef>().Where(x => x.Name == variable.Name).FirstOrDefault();
            if (def != null)
                List.Remove(variable);
        }

        public bool Remove(KeyValuePair<string, WFVariableDef> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(string key, out WFVariableDef value)
        {
            WFVariableDef v;
            bool ret = dic.TryGetValue(key, out v);
            value = v;
            return ret;
        }

        protected override void OnClearComplete()
        {
            foreach (var item in dic.Select(x => x.Key).ToArray())
                Remove(item);
        }

        protected override void OnInsertComplete(int index, object value)
        {
            var variable = value as WFVariableDef;
            if (variable != null)
                Add(variable);
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            WFVariableDef variable = (WFVariableDef)value;
            if (dic.ContainsKey(variable.Name))
                Remove(variable.Name);

        }
    }
}
