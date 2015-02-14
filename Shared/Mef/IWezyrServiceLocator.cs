using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace SoftwareMind.Shared.Mef
{
    public interface IWezyrServiceLocator : IServiceLocator
    {
        new object GetInstance(Type serviceType);
        new object GetInstance(Type serviceType, string key);
        object GetInstance(Type serviceType, params KeyValuePair<string, object>[] metaData);
        new IEnumerable<object> GetAllInstances(Type serviceType);

        new TService GetInstance<TService>();
        new TService GetInstance<TService>(string key);
        TService GetInstance<TService>(params KeyValuePair<string, object>[] metaData);
        new IEnumerable<TService> GetAllInstances<TService>();

        Lazy<TService> GetLazy<TService>();
        Lazy<TService> GetLazy<TService>(string key);
        Lazy<TService> GetLazy<TService>(params KeyValuePair<string, object>[] metaData);
    }
}
