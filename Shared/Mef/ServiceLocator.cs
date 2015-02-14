using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace SoftwareMind.Shared.Mef
{
    public static class ServiceLocator
    {
        public static void SetLocatorProvider(ServiceLocatorProvider provider, bool cache = true)
        {
            IServiceLocator locator = null;
            ServiceLocatorProvider cacheProvider = () => locator ?? (locator = provider());

            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(cache ? cacheProvider : provider);
        }

        public static IWezyrServiceLocator Current
        {
            get { return (IWezyrServiceLocator) Microsoft.Practices.ServiceLocation.ServiceLocator.Current; }
        }
    }
}
