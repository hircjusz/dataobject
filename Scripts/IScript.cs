using System;
namespace SoftwareMind.Scripts
{
    public interface IScript
    {
        object Execute();
        object this[string name] { get; set; }
        void Validate(System.Collections.Generic.IDictionary<string, Type> typeDic);
    }
}
