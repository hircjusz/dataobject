using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects.NET;

namespace SoftwareMind.Shared.Infrastructure
{
    public interface ISessionWrapperProvider
    {
        ISessionWrapper GetWrapper();
    }

    public interface ISessionWrapper : IDisposable
    {
        Session Session { get; }
    }
}
