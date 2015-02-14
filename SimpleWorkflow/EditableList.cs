using System;
using System.Collections;
using System.Collections.Generic;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class EditableList<T>:  CollectionBase, IList<T>
    {
        public T this[int index]
        {
            get
            {
                return ((T)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(T value)
        {
            return (List.Add(value));
        }

        public int IndexOf(T value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, T value)
        {
            List.Insert(index, value);
        }

        public void Remove(T value)
        {
            List.Remove(value);
        }

        public bool Contains(T value)
        {
            return (List.Contains(value));
        }

        protected override void OnValidate(Object value)
        {
            if (!value.GetType().IsAssignableFrom(typeof(T)))
                throw new ArgumentException("Typ obiektu musi być typu: " + typeof(T).FullName, "value");
        }


        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get 
            { 
                return List.IsReadOnly; 
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            if (List.Contains(item))
            {
                List.Remove(item);
                return true;
            }
            else
                return false;
        }

        public new IEnumerator<T> GetEnumerator()
        {
            foreach (var item in List)
                yield return (T)item;
        }
    }
}
