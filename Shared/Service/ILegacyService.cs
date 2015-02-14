using System;
using DataObjects.NET;
using SoftwareMind.Shared.Dto;
using SoftwareMind.Shared.Infrastructure;

namespace SoftwareMind.Shared.Service
{
    public interface ILegacyService : IService
    {
        void Invoke(Action<Session> func);
        T Invoke<T>(Func<Session, T> func);

        IContainer InvokeSelect<T>(AccessStoreActionDTO<T> action, Func<Session, IStatement> func);
        IContainer InvokeUpdate<T>(ModifyStoreActionDTO<T> action);
    }
}
