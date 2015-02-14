using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using SoftwareMind.Shared.Mef;

namespace SoftwareMind.Utils
{
    internal class Injector
    {
        private static readonly IDictionary<Type, MemberInfo[]> cachedMembers = new Dictionary<Type, MemberInfo[]>();

        internal void InjectAnnotated(object o, Type t)
        {
            if (t == null || t == typeof(object) || t.IsPrimitive)
            {
                return;
            }

            MemberInfo[] members;
            lock (cachedMembers)
            {
                if (!cachedMembers.ContainsKey(t))
                {
                    var m = t.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty)
                        .Where(p => p.GetCustomAttributes(typeof(ImportAttribute), true).Any())
                        .ToArray();

                    cachedMembers.Add(t, members = m);
                }
                else
                {
                    members = cachedMembers[t];
                }
            }
            foreach (var p in members.OfType<PropertyInfo>())
            {
                if (p.GetValue(o, null) == null)
                {
                    object value = this.GetImportedValue(p, p.PropertyType);
                    if (value != null)
                    {
                        p.SetValue(o, value, null);
                    }
                }
            }
            foreach (var f in members.OfType<FieldInfo>())
            {
                if (f.GetValue(o) == null)
                {
                    object value = this.GetImportedValue(f, f.FieldType);
                    if (value != null)
                    {
                        f.SetValue(o, value);
                    }
                }
            }

            this.InjectAnnotated(o, t.BaseType);
        }

        private object GetImportedValue(MemberInfo memberInfo, Type memberType)
        {
            var importAttribute = memberInfo.GetCustomAttributes(typeof(ImportAttribute), true).Cast<ImportAttribute>().Single();
            var importType = importAttribute.ContractType ?? memberType;

            return importAttribute.ContractName == null ?
                ServiceLocator.Current.GetInstance(importType) :
                ServiceLocator.Current.GetInstance(importType, importAttribute.ContractName);
        }
    }

    public static class InjectorHelper
    {
        private static readonly Injector injector = new Injector();

        public static T InjectAnnotated<T>(this T o)
        {
            injector.InjectAnnotated(o, o.GetType());
            return o;
        }
    }
}
