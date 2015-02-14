using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareMind.Shared.Infrastructure
{
    public struct NamedFilterResult
    {
        public string Condition { get; private set; }
        public IList<Tuple<string, object>> Parameters { get; private set; }

        public NamedFilterResult(string condition, IEnumerable<Tuple<string, object>> parameters) : this()
        {
            Condition = condition;
            Parameters = parameters.ToList();
        }

        public NamedFilterResult(string condition, params Tuple<string, object>[] parameters) : this()
        {
            Condition = condition;
            Parameters = parameters;
        }
    }

    public class NamedFilter
    {
        public delegate NamedFilterResult Func(IContext context);

        public Type Type { get; private set; }
        public string Name { get; private set; }
        public Func Method { get; private set; }

        public NamedFilter(Type type, string name, Func method)
        {
            Method = method;
            Name = name;
            Type = type;
        }
    }

    public interface INamedFilterProvider
    {
        NamedFilterResult InvokeFilterByName(Type entity, string name);
    }
}